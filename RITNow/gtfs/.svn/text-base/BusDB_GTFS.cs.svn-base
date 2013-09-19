using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using MonoTouch.CoreLocation;
using LumenWorks.Framework.IO.Csv;

namespace RITNow
{
	/// <summary>
	/// Interface with underlying data structure to get route and schedule info.
	/// Singleton since there is only one set of data and we want to minimize storing in memory.
	/// </summary>
	public class BusDB_GTFS_SQL_GTFS
	{

		XElement indexRoot;
		private static BusDB_GTFS_SQL_GTFS instance;
		private const string DATA_DIR = "data";
		public string AuthorityDir = "RGRTA_gtfs";

		//singleton - get instance
		public static BusDB_GTFS_SQL_GTFS Instance {
			//TODO: Sure this should be singleton?
			get {
				if (instance == null)
					instance = new BusDB_GTFS_SQL_GTFS ();
				return instance;
			}
		}

		//initialize by loading the index
		private BusDB_GTFS_SQL_GTFS ()
		{
			//TODO: What if there's no index file (yet to downlaod?)

		}

		//Caution: If radius is too large results will be HUGE!
		//radius in meters
		public IEnumerable<BusStop> getAllBusStops (CLLocation aboutLoc, double radius)
		{
			List<BusStop> stops = new List<BusStop>(30);
			using (LumenWorks.Framework.IO.Csv.CsvReader csv = new LumenWorks.Framework.IO.Csv.CsvReader(getReader("stops.txt"),true)) {
				while (csv.ReadNextRecord()){
					CLLocation loc = new CLLocation(double.Parse(csv["stop_lat"]), double.Parse(csv["stop_lon"]));
					if (loc.DistanceFrom(aboutLoc)<=radius)
						stops.Add(new BusStop(csv["stop_name"],int.Parse(csv["stop_id"]), loc));
					loc.Dispose();
				}
				return stops;
			}
		}

		public BusStop? getClosestStop (CLLocation aboutLoc)
		{
			int closestId = -1;
			double closestDist=double.MaxValue;
			using (CsvReader csv = new CsvReader(getReader("stops.txt"),true)) {
				while (csv.ReadNextRecord()){
					CLLocation loc = new CLLocation(double.Parse(csv["stop_lat"]), double.Parse(csv["stop_lon"]));
					if (loc.DistanceFrom(aboutLoc)<=closestDist)
						closestId=int.Parse(csv["stop_id"]);
					loc.Dispose();
				}
			}
			return (closestId==-1)?null:getStopInfo(closestId);
		}

		public Nullable<BusStop> getStopInfo (int id)
		{
			using (LumenWorks.Framework.IO.Csv.CsvReader csv = new LumenWorks.Framework.IO.Csv.CsvReader(getReader("stops.txt"),true)) {
				while (csv.ReadNextRecord()){
					if (int.Parse(csv["stop_id"])== id){
						return new BusStop(csv["stop_name"],id, new CLLocation(double.Parse(csv["stop_lat"]), double.Parse(csv["stop_lon"])));
					}
				}
				return null;
			}
				
		}

		public IEnumerable<Arrival> getNextArrivals (int stop,TimeSpan maxFuture,DateTime afterTime)
		{
			return null;
		}

		private StreamReader getReader (string File)
		{
			return new StreamReader (System.IO.Path.Combine (DATA_DIR, AuthorityDir, File));
		}
	}
}

