using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using MonoTouch.CoreLocation;
using LumenWorks.Framework.IO.Csv;
using SQLite;

namespace RITNow
{
	/// <summary>
	/// Interface with underlying data structure to get route and schedule info.
	/// Singleton since there is only one set of data and we want to minimize storing in memory.
	/// </summary>
	public class BusDB_GTFS_SQL
	{

		private static BusDB_GTFS_SQL instance;
		private const string DATA_DIR = "data";
		private string dataFile = "RGRTA.sqlite";
		private SQLiteConnection db;
		private readonly object dbLock = new object();

		//singleton - get instance
		public static BusDB_GTFS_SQL Instance {
			//TODO: Sure this should be singleton?
			get {
				if (instance == null)
					instance = new BusDB_GTFS_SQL ();
				return instance;
			}
		}

		//initialize by loading the index
		private BusDB_GTFS_SQL ()
		{
			//TODO: What if there's no index file (yet to downlaod?)
			string dbFile = Path.Combine (DATA_DIR, dataFile);
			if (!File.Exists (dbFile))
				throw new Exception ("no database file");
			else {
				db = new SQLiteConnection (dbFile);
			}
		}

		//Caution: If radius is too large results will be HUGE!
		//radius in meters
		public IEnumerable<BusStop> getAllBusStops (CLLocation aboutLoc, double radius)
		{
			var result = from s in SafeQuery<GTFS_Stop>("select * from stops")
				let loc = new CLLocation(s.stop_lat,s.stop_lon)
				where (loc.Distancefrom(aboutLoc)<radius)
					select new BusStop(s.stop_name,s.stop_id,loc);
			return result;
		}
		public IEnumerable<BusStop> getAllBusStops ()
		{
			var result = from s in SafeQuery<GTFS_Stop>("select * from stops")
				let loc = new CLLocation(s.stop_lat,s.stop_lon)
					select new BusStop(s.stop_name,s.stop_id,loc);
			return result;
		}

		//a more memory friendly way to get the closest stop than getting a list, sorting, and tossing
		public BusStop? getClosestStop (CLLocation aboutLoc)
		{
			var allStops = SafeQuery<GTFS_Stop>("select * from stops");
			int closestId = -1;
			double closestDist=double.MaxValue;
			foreach (GTFS_Stop stop in allStops){
				CLLocation loc = new CLLocation(stop.stop_lat,stop.stop_lon);
					if (loc.DistanceFrom(aboutLoc)<=closestDist)
						closestId=stop.stop_id;
				loc.Dispose();
			}
			return (closestId==-1)?null:getStopInfo(closestId);
		}

		public BusStop? getStopInfo (int id)
		{
			var result = SafeQuery<GTFS_Stop>("select * from stops where stop_id = ?",id);
			if (result.Count==0)
				return null;
			GTFS_Stop gtfsStop = result.Single();
			return new BusStop(gtfsStop.stop_name, gtfsStop.stop_id, new CLLocation(gtfsStop.stop_lat,gtfsStop.stop_lon));
		}

		//get the next arrivals for a stop within a window from [afterTime, afterTime+maxFuture]
		public IEnumerable<Arrival> getNextArrivals (int stop, TimeSpan maxFuture, DateTime afterTime)
		{
			lock (dbLock) {

				var results = from s in SafeQuery<GTFS_Stop_Time> ("select * from stop_times where stop_id = ?", stop)
				let trip = (GTFS_Trip)getTrip (s.trip_id)

				//the clock time of the arrival in hours since midnight (or 12hr before noon) on the start of the trip
				let arrivalTime = parseTime (s.arrival_time)
				
				//if the trip started yesterday, then the arrivalTime will not be from the last midnight
				let startedYesterday = arrivalTime.Hours > 24

				//if started yesterday, arrival is from midnight yesterday, otherwise from midnight today
				//TODO: Actually it should be noon minus 12 hours (not always midnight), but that's just crazy
				let timeToday = startedYesterday ? afterTime.AddDays (-1).Date.Add (arrivalTime) : afterTime.Date.Add (arrivalTime)
								
				//in the window and running today
				where (timeToday >= afterTime && timeToday <= afterTime.Add (maxFuture)) && runningToday(startedYesterday,afterTime, trip)
				//TODO: The BusRoute is really unnessesary for this program, as we only care about the headsign
				select new Arrival ((BusStop)getStopInfo (stop), (BusRoute)getBusRoute (trip.route_id), timeToday, trip.trip_headsign);
				return results;//TODO: Convert to arrival
			}	
		}



		/* HELPERS*/


		private GTFS_Trip getTrip (int trip_id)
		{
			var result =SafeQuery<GTFS_Trip>("select * from trips where trip_id = ?", trip_id);
			if (result.Count==0)
				return null;
			else
				return result.Single();
		}
		private BusRoute? getBusRoute (int route_id)
		{
			var result =SafeQuery<GTFS_Route>("select * from routes where route_id = ?", route_id);

			GTFS_Route gtfsRoute;
			if (result.Count==0)
				return null;
			else
				gtfsRoute= result.Single();
			return new BusRoute(gtfsRoute.route_short_name, gtfsRoute.route_long_name);
		}

		//check whether service was valid on the day the trip started (usually today unless startedYesterday)
		private bool runningToday(bool startedYesterday, DateTime day, GTFS_Trip trip){
			return startedYesterday ? runsOnDay (trip.service_id, day.AddDays (-1)) : runsOnDay (trip.service_id, day);
		}


		private bool runsOnDay (int service_id, DateTime afterTime)
		{

			//FIRST LOOK AT EXCEPTIONS
				var exceptions = SafeQuery<GTFS_Calendar_Date> ("select * from calendar_dates where service_id = ? and date = ?", service_id, afterTime.ToString ("yyyyMMdd"));

			if (exceptions.Count == 1) {//there is an exception
				GTFS_Calendar_Date exception = exceptions.Single();
				//1 is service has been added, 2 has been removed
				if (exception.exception_type==GTFS_Calendar_Date.ExceptionType.ADDED)
					return true;//exception added this day - it runs
				else if (exception.exception_type==GTFS_Calendar_Date.ExceptionType.REMOVED)
					return false;//exception removed this day - it does not run
			}

			//If the exceptions didn't apply, look at the standard calendar

			GTFS_Calendar service = SafeQuery<GTFS_Calendar> ("select * from calendar where service_id = ?", service_id).Single ();

			bool ordinarilyRuns = false;//runs based on 'calendar' only (not calendar_dates)
			switch (afterTime.DayOfWeek) {//check that it runs on the day being tested
			case DayOfWeek.Monday:
				ordinarilyRuns = service.monday == 1;
				break;
			case DayOfWeek.Tuesday:
				ordinarilyRuns = service.tuesday == 1;
				break;
			case DayOfWeek.Wednesday:
				ordinarilyRuns = service.monday == 1;
				break;
			case DayOfWeek.Thursday:
				ordinarilyRuns = service.thursday == 1;
				break;
			case DayOfWeek.Friday:
				ordinarilyRuns = service.friday == 1;
				break;
			case DayOfWeek.Saturday:
				ordinarilyRuns = service.saturday == 1;
				break;
			case DayOfWeek.Sunday:
				ordinarilyRuns = service.sunday == 1;
				break;
			}
			//it also has to valid in range
			ordinarilyRuns = ordinarilyRuns && (afterTime > parseDate (service.start_date) && afterTime < parseDate (service.end_date));
			return ordinarilyRuns;

	
		}
		private DateTime parseDate (string gtfsDate)
		{
			return DateTime.ParseExact(gtfsDate, "yyyyMMdd", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
		}

		//TODO: Make me private (testing)
		public TimeSpan parseTime (string timeS)
		{
			string[] timeParts = timeS.Split (':');
			return new TimeSpan(int.Parse(timeParts[0]), int.Parse(timeParts[1]),int.Parse(timeParts[2]));
		}

		 private List<T> SafeQuery<T> (string query, params object[] args) where T : new()
		{
			Console.WriteLine("QUERY: "+query);
			lock (dbLock) {
				return db.Query<T> (query, args);
			}
		}

	}

	public class GTFS_Stop{
		[PrimaryKey]
		public int stop_id { get; set; }
		public string stop_code { get; set; }
		public string stop_name { get; set; }
		public string stop_desc { get; set; }
		public double stop_lat { get; set; }
		public double stop_lon { get; set; }
		public int zone_id { get; set; }
		public string stop_url { get; set; }
		public int location_type { get; set; }
		public int parent_station { get; set; }
		public string stop_timezone { get; set; }
		public int wheelchair_boarding { get; set; }

	}
	public class GTFS_Stop_Time{
		public int trip_id { get; set; } 
		public string arrival_time { get; set; } 
		public string departure_time { get; set; }
		public int stop_id { get; set; }
		public int stop_sequence { get; set; }
		public string stop_headsign { get; set; }
		public int pickup_type { get; set; }
		public int drop_off_type { get; set; }
		public double shape_dist_traveled { get; set; }
	}
	public class GTFS_Trip{
		public int route_id { get; set; } 
		public int service_id { get; set; } 
		[PrimaryKey]
		public int trip_id { get; set; } 
		public string trip_headsign { get; set; } 
		public int trip_short_name { get; set; } 
		public bool direction_id { get; set; }
		public int block_id { get; set; } 
		public int shape_id { get; set; } 
		public int wheelchair_accessible { get; set; } 

	}
	public class GTFS_Calendar{
		[PrimaryKey]
		public int service_id { get; set; } 
		public int monday { get; set; }
		public int tuesday { get; set; }
		public int wednesday { get; set; }
		public int thursday { get; set; }
		public int friday { get; set; }
		public int saturday { get; set; }
		public int sunday { get; set; }
		public string start_date { get; set; }
		public string end_date { get; set; }
	}
	public class GTFS_Calendar_Date{
		public int service_id { get; set; } 
		public string date { get; set; } 
		public ExceptionType exception_type { get; set; } 
		public enum ExceptionType{
			ADDED=1,
			REMOVED=2
		}
	}
	public class GTFS_Route{
		[PrimaryKey]
		public int route_id { get; set; } 
		public int agency_id { get; set; } 
		public string route_short_name { get; set; } 
		public string route_long_name { get; set; }
		public string route_desc { get; set; } 
		//public int route_type { get; set; } 
		//url, color, text omitted
	}
}

