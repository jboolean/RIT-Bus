using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.IO;
using MonoTouch.CoreLocation;

namespace RITNow
{
	/// <summary>
	/// Interface with underlying data structure to get route and schedule info.
	/// Singleton since there is only one set of data and we want to minimize storing in memory.
	/// </summary>
	public class BusDB
	{

		XElement indexRoot;
		private static BusDB instance;
		private const string DATA_DIR = "data";
		private static string INDEX_FILENAME = System.IO.Path.Combine (DATA_DIR, "Index.xml");

		//singleton - get instance
		public static BusDB Instance {
			get {
				if (instance == null)
					instance = new BusDB ();
				return instance;
			}
		}

		//initialize by loading the index
		private BusDB ()
		{
			//TODO: What if there's no index file (yet to downlaod?)
			indexRoot = XDocument.Load (INDEX_FILENAME).Element("RouteIndex");
		}

		public IEnumerable<BusStop> getAllBusStops ()
		{
			return from destNode in indexRoot.Element ("Destinations").Elements ("Destination")
				select this.getBusStopFromDestinationNode (destNode);
		}

		public Nullable<BusStop> getStopInfo (int id)
		{
			try {
				XElement destNode = getDestinationNode (id);
				return this.getBusStopFromDestinationNode (destNode);
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				return null;
			} 

		}
		//should be private
		public BusStop getBusStopFromDestinationNode (XElement destNode)
		{
			if (destNode==null)
				throw new NullReferenceException("Destination node cannot be null");
			return new BusStop (destNode.Element ("Name").Value, 
				                       int.Parse (destNode.Attribute ("stopId").Value), 
				                       new CLLocation (double.Parse (destNode.Element ("Location").Attribute ("lat").Value), 
				                           double.Parse (destNode.Element ("Location").Attribute ("lon").Value))
			);
		}

		private XElement getDestinationNode (int id)
		{
			try {
				return (from c in indexRoot.Element ("Destinations")
                                .Elements ("Destination")
                            where (int.Parse(c.Attribute ("stopId").Value) == id)
			                            select c).Single ();
			} catch (InvalidOperationException e) {
				Console.WriteLine(e.Message);
				return null;
			} 
		}

		public IEnumerable<Arrival> getNextArrivals (int stop,TimeSpan maxFuture,DateTime afterTime)
		{
		
			BusStop? ourStop = this.getStopInfo (stop);
			if (!ourStop.HasValue)
				return null;

			//get list of route names from this stop
			var routes = from busRoute in getDestinationNode (stop).Elements ("BusRoute")
				select new BusRoute (busRoute.Attribute ("fullName").Value);
			//attempt to load files named ROUTE-1,ROUTE-2 until it fails
			//build a list of relevant arrivals
			List<Arrival> arrivalList = new List<Arrival> ();
			foreach (BusRoute bus in routes) {
				int i = 1;
				string fileName;
				while (File.Exists(fileName = Path.Combine (DATA_DIR, bus.FullName + "-" + i + ".xml"))) {
					XElement busFile = (XDocument.Load (fileName)).Element("BusRoute");
					//if it runs today get the time to/since each time it hits our stop. 
					if (this.runs (busFile.Element ("ActiveDays"), afterTime)) {
						foreach (XElement stopNode in busFile.Element("Timetable").Elements("Stop")) {
							//only look at times at the relevant stop (here)
							if (int.Parse (stopNode.Element ("StopId").Value) == stop) {
								TimeSpan timeOfDay = TimeSpan.Parse(stopNode.Element ("Time").Value);
								DateTime timeOnDay = afterTime.Date.Add (timeOfDay);
								//if in the future within maxtime add it to a list
								if (timeOnDay >= afterTime && timeOnDay < (afterTime.Add (maxFuture))) {
									arrivalList.Add (new Arrival ((BusStop)ourStop, bus, timeOnDay));
								}//end time in timeframe
							}//end is our stop
						}//end foreach stop
					}//end runs today
					i++;//increment the file suffix
				}//end try every file
			}//end each possible busRoute
			return arrivalList;
		}//end method


		
//		public IEnumerable<BusRoute> getRoutesBetween (Destination origin, Destination destination)
//		{
//			//all routes
//			var routesAsXml = from route in indexRoot.Element ("BusRoutes").Elements ("BusRoute")
//				where containsStop (route, origin.stopId) && containsStop (route, destination.stopId)
//				select route;
//			List<BusRoute> toReturn = new List<BusRoute>();
//			foreach (XElement route in routesAsXml) {//build into BusRoute structs from XML
//				BusRoute busRoute = new BusRoute();
//				busRoute.Number=route.Attribute("number").Value;
//				busRoute.FullName=route.Attribute("name").Value;
//				var stops = from stopNode in route.Element("Destinations").Descendants("StopId")
//					select getDestination(int.Parse(stopNode.Value));
//				busRoute.stops=stops.ToArray();
//				toReturn.Add(busRoute);
//			}
//			return toReturn;
//		}
//
//		/// <summary>
//		/// Gets a list of places you can go from here.
//		/// </summary>
//		public List<Destination> getDestinationsFrom (Destination origin)
//		{
//			var destinationsElements = from route in indexRoot.Element("BusRoutes").Elements("BusRoute")
//				where containsStop(route,origin.stopId)
//					select route.Element("Destinations");
//			List<int> stopIds = new List<int>();//temporary list to easily avoid duplicates
//			//don't duplicate origin
//			stopIds.Add (origin.stopId);
//
//			//pull stopIds out of each bus list
//			List<Destination> destinationsToReturn = new List<Destination>();
//			foreach (XElement destinationsElement in destinationsElements){
//				foreach (XElement stopIdElement in destinationsElement.Descendants("StopId")){
//					int id = int.Parse(stopIdElement.Value);
//					if (!stopIds.Contains(id)){
//						stopIds.Add(id);
//						destinationsToReturn.Add(this.getDestination(id));//convert to Destination struct
//					}
//				}
//			}
//		}
//				 
//		//Does a BusRoute node in index contain a stop. 		                 
//		private static bool containsStop(XElement route, int stopId){
//			foreach (XElement stop in route.Element("Destinations").Elements("StopId")){
//				if (int.Parse(stop.Value) == stopId)
//					return true;
//			}
//			return false;
//		}
//		public List<BusTrip> getTodaysTrips (Destination origin, Destination destination)
//		{
//			/*
//			 * get routes between the destinations
//			 * for each route
//			 * check if it runs today
//			 * if it does go to each stop matching origin
//			 * from there find the next time to goes to the destination
//			 * build a BusTrip
//			 */
//			foreach (string busFile in this.getBusFileNames(getRoutesBetween(origin,destination))) {
//				XDocument busRoot = XDocument.Load (busFile);
//				if (runsToday(busRoot.Element("ActiveDays"))){
//
//					//not getting destination list for now because it's not needed
//					BusRoute route = new BusRoute(busRoot.Element("RouteNumber").Value,busRoot.Element("RouteName").Value,null);
//
//
//				}
//			}
//		}

		static DayOfWeek[] weekdays = {DayOfWeek.Friday, DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Tuesday, DayOfWeek.Wednesday};
		static DayOfWeek[] weekends = {DayOfWeek.Saturday, DayOfWeek.Sunday};

		public bool isAHoliday (DateTime day)
		{
			return false;
		}

		private bool runs (XElement activeDaysNode, DateTime day)
		{
			DayOfWeek thisWeekday = day.DayOfWeek;
			if (weekdays.Contains(thisWeekday)&&activeDaysNode.Element("Weekdays")!=null&&activeDaysNode.Element("Weekdays").Value=="0")
				return false;
			if (weekends.Contains(thisWeekday)&&activeDaysNode.Element("Weekends")!=null&&activeDaysNode.Element("Weekdays").Value=="0")
				return false;
			if (isAHoliday(day)&&activeDaysNode.Element("Holidays")!=null&&activeDaysNode.Element("Holidays").Value=="0")
				return false;
			return true;
		}
//

//
//		/// <summary>
//		/// Gets the destination struct from a stopId
//		/// </summary>
//		/// <returns>
//		/// The destination or an empty destination
//		/// </returns>
//		/// <param name='stopId'>
//		/// Stop identifier.
//		/// </param>
//		public Destination getDestination (int stopId)
//		{
//			IEnumerable<Destination> allMatchingDestinations = from destNode in indexRoot.Element("Destinatons").Elements("Destination")
//				where destNode.Attribute("stopId").Value==stopId.ToString
//					select new Destination(destNode.Element("Name").Value, 
//					                       int.Parse(destNode.Attribute("stopId")),
//					                       new CLLocationCoordinate2D(destNode.Element("Location").Attribute("lat"), 
//				                           destNode.Element("Location").Attribute("lon"))
//				                       );
//			//there should only one element in that list
//			return allMatchingDestinations.FirstOrDefault();
//		}
//		private string[] getBusFileNames (BusRoute route)
//		{
//			var routeNodes = from routeNode in indexRoot.Element("BusRoutes").Elements("BusRoute")
//				where routeNode.Attribute("number")==route.Number
//					select routeNode;
//
//			IEnumerable<string> fileNames = from fileNode in routeNodes.First().Elements("BusFile")
//				select fileNode.Value;
//			return fileNames.ToArray();
//			//TODO: will fail if no such route
//		}
//		public static bool IsAHoliday(DateTime day){
//			//TODO
//			return false;
//		}
	}
}

