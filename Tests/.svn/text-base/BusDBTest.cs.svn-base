
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
	public class BusDBTest
	{
		BusDB db;

		[SetUp]
		public void Init(){
			db=BusDB.Instance;
		}

		[Test]
		public void testGetBusStopFromDestinationNode ()
		{
			var node = XElement.Parse("<Destination stopId=\"9999\"> " +
				"<Name>Unit Test/Stuff</Name> " +
				"<Location lat=\"40.000000\" lon=\"-70.000000\"/> " +
				"<BusRoute number=\"28\" fullName=\"Route 1 to Unit Test\"/> " +
				"</Destination>");
			BusStop stop = db.getBusStopFromDestinationNode(node);
			Assert.That(stop.name== "Unit Test/Stuff");
			Assert.That(stop.stopId== 9999);
			Assert.That(stop.location.Coordinate.Latitude== 40.000000);
			Assert.That(stop.location.Coordinate.Longitude== -70.000000);

		}

		[Test]
		public void testGetAllBusStops(){
			Assert.AreEqual(db.getAllBusStops().First().stopId,3382);
			Assert.That(db.getAllBusStops().Count()==1);
		}

		[Test]
		public void testGetStopInfo ()
		{
			BusStop? validStop = db.getStopInfo(3382);
			Assert.That(validStop.HasValue);
			Assert.AreEqual(((BusStop)validStop).name,"Colony Manor");
			Assert.AreEqual(((BusStop)validStop).stopId,3382);
			Assert.That(((BusStop)validStop).location.Coordinate.Latitude== 43.083023);
			Assert.That(((BusStop)validStop).location.Coordinate.Longitude== -77.658421);

			Assert.False(db.getStopInfo(1111).HasValue);

		}

		[Test]
		public void testGetNextArrivals(){
			var arrs = db.getNextArrivals(3382,new TimeSpan(1,0,0,0), DateTime.Today);
			var arrival = arrs.First();
			Assert.That(arrs.Count()==1, "Should be 1 arrival");
			Assert.That(arrival.Time==DateTime.Today.Add(new TimeSpan(23,59,59)));
		}

		[Test]
		public void testCreateRoute(){
			BusRoute r = new BusRoute("Route 222 to Bridge to Nowhere");
			Assert.AreEqual("Bridge to Nowhere", r.Long_Name);
			Assert.AreEqual("222",r.Short_Name);
		}

		/*[Test]
		public void testIsAHoliday ()
		{
		}*/
	}
}
