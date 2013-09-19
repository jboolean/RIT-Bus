
using System;
using NUnit.Framework;

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
		public void testGetAllBusStops(){

		}

		[Test]
		public void testGetStopInfo ()
		{

		}

		[Test]
		public void testGetNextArrivals(){

		}
	}
}
