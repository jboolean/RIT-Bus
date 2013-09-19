
using System;
using NUnit.Framework;
using RITNow;

namespace Tests
{
	[TestFixture]
	public class LiveArrivalFetcherTest
	{
		LiveArrivalFetcher fetcher;
		[SetUp]
		public void SetUp(){
			fetcher = new LiveArrivalFetcher(new BusStop("Colony Manor", 3382, new MonoTouch.CoreLocation.CLLocation(40,-70)));
		}

		[Test]
		public void fetchertest1 ()
		{
			var results = fetcher.doFetchNow();
			foreach (var result in results)
				Console.WriteLine("ARRIVAL: "+result.ToString());
			Assert.IsTrue(results.Count>0);
		}

		[Test]
		public void testParseRtsTime(){

			Assert.That(DateTime.Today.Add(new TimeSpan(23,59,00)).Equals(LiveArrivalFetcher.parseRtsTime("11:59 PM")));
			//Assert.That(DateTime.Now.AddMinutes(1.0).Equals(LiveArrivalFetcher.parseRtsTime("DUE")));
			//Assert.That(DateTime.Now.AddMinutes(2).Equals(LiveArrivalFetcher.parseRtsTime("1 min(s)")));
		}

	}
}
