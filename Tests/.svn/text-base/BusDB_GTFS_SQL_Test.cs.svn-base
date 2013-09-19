
using System;
using NUnit.Framework;
using RITNow;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
namespace RITNow
{
	[TestFixture]
	public class BusDB_GTFS_SQL_Test
	{
		BusDB_GTFS_SQL db;

		[SetUp]
		public void Init(){
			db=BusDB_GTFS_SQL.Instance;
		}


		[Test]
		public void testGetAllBusStops(){
//			Assert.AreEqual(db.getAllBusStops().First().stopId,3382);
//			Assert.That(db.getAllBusStops().Count()==1);
			var loc =new MonoTouch.CoreLocation.CLLocation(43.083023, -77.658421);
			var stops = db.getAllBusStops(loc,100);
			Assert.That(stops.Contains(new BusStop("Colony Manor", 3382, loc)));
		}

		[Test]
		public void testGetStopInfo ()
		{
			BusStop? validStop = db.getStopInfo(3382);
			Assert.That(validStop.HasValue);
			Assert.AreEqual("RIT Colony Manor",((BusStop)validStop).name);
			Assert.AreEqual(3382,((BusStop)validStop).stopId);
			Assert.AreEqual(43.083023,((BusStop)validStop).location.Coordinate.Latitude);
			Assert.AreEqual(-77.658421,((BusStop)validStop).location.Coordinate.Longitude);

			Assert.False(db.getStopInfo(int.MaxValue).HasValue);

		}

		[Test]
		public void testGetNextArrivals(){
			var arrs = db.getNextArrivals(3382,new TimeSpan(1,0,0), new DateTime(2013,1,14,12,0,0));
			foreach (var arr in arrs)//I"M a pirate arrrrr
				Console.WriteLine("SCHED ARRIVAL: "+arr);
			Assert.Pass();
		}

		[Test]
		public void testCreateRoute(){
			BusRoute r = new BusRoute("Route 222 to Bridge to Nowhere");
			Assert.AreEqual("Bridge to Nowhere", r.Long_Name);
			Assert.AreEqual("222",r.Short_Name);
		}

		[Test]
		public void testParseTime(){
			Assert.AreEqual(new TimeSpan(1,23,45),db.parseTime("1:23:45"));
			Assert.AreEqual(new TimeSpan(12,23,45),db.parseTime("12:23:45"));
			Assert.AreEqual(new TimeSpan(24,23,45),db.parseTime("24:23:45"));
		}

		/*[Test]
		public void testIsAHoliday ()
		{
		}*/
	}
}
