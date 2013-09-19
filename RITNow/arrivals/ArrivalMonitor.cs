using System;
using System.Collections.Generic;
using System.Linq;

namespace RITNow
{
	/// <summary>
	/// An layer between the schedule/live subsystem and a client
	/// Acts as a kind of facade to the schedule subsystem
	/// Doesn't do anything until RefreshResults is called.
	/// Upon notification on ArrivalsUpdated, the Results property can be accessed.
	/// Will fallback to schedule data when network unavailable or an error occurs with live fetching.
	/// Will return schedule data while live is processing.
	/// </summary>
	public class ArrivalMonitor
	{
		/// <summary>
		/// Occurs when arrivals may be updated due to new data from live, 
		/// or reoccuring interval or on demand has returned results
		/// </summary>
		public event ArrivalObserver ArrivalsUpdated;
		public delegate void ArrivalObserver (ArrivalMonitor m,ArrivalEvent e);

		private BusStop myStop;
		/// <summary>
		/// The after time in scheduled mode, or null to always mean NOW
		/// </summary>
		private DateTime afterTime;
		private TimeSpan aheadTime;
		private List<Arrival> results;
		private BusDB_GTFS_SQL db;
		private LiveArrivalFetcher fetcher;
		private bool liveMode = false;


		/// <summary>
		/// Use schedules only
		/// </summary>
		public ArrivalMonitor (BusStop stop, DateTime afterTime)
		{
			myStop = stop;
			this.afterTime = afterTime;
			this.aheadTime = new TimeSpan (2, 0, 0);//default look ahead to 2 hrs
			db = BusDB_GTFS_SQL.Instance;

			//future time is close enough to now that RTS may have data 
//			if (afterTime.Subtract (DateTime.Now) < new TimeSpan (2, 0, 0))
//				startLiveFetcher ();
		}
		/// <summary>
		/// Get data from NOW including LIVE data (when network online)
		/// </summary>
		public ArrivalMonitor (BusStop stop) : this(stop, DateTime.Now)
		{
			liveMode = true;
			db = BusDB_GTFS_SQL.Instance;
			fetcher = new LiveArrivalFetcher (myStop);
			fetcher.ArrivalsUpdated += new LiveArrivalFetcher.ArrivalObserver (receiveLiveResults);
		}

		public List<Arrival> getDataScheduleNow ()
		{
			List<Arrival> scheduled = db.getNextArrivals (myStop.stopId, aheadTime, (liveMode) ? DateTime.Now : afterTime).ToList ();
			scheduled.Sort ();
			return scheduled;
		}

		
		/// <summary>
		/// Asynchronous. Get results when notified by the ArrivalsUpdated event.
		/// In live mode, will request that the fetcher get data.
		/// In scheduled mode, will make a database call.
		/// </summary>
		public void RefreshResults ()
		{
			if (liveMode /*&& LiveArrivalFetcher.Reachable*/) {//live mode
				fetcher.DoFetchBackground ();
				if (this.Results == null)
					fetchScheduleDataBackground ();
			} else {
				fetchScheduleDataBackground ();
			}
		}

		private void fetchScheduleDataBackground ()
		{
			System.Threading.Tasks.Task.Factory.StartNew (() => {
				results = getDataScheduleNow ();
				notify ();
			}
			);
		}

		/// <summary>
		/// USED IN LIVE MODE
		/// Receives the live results from the live fetcher
		/// reconciled with scheduled data and notifies observers
		/// </summary>
		private void receiveLiveResults (LiveArrivalFetcher fetcher, ArrivalEvent e)
		{
			if (fetcher != this.fetcher || !liveMode)//verify this is the correct fetcher
				return;
			List<Arrival> masterList = new List<Arrival> ();
			if (e.Error) {//don't propagate the live error (whatever it was). Just return only the schedule data.
				results = getDataScheduleNow ();
				notify ();
				return;
			}
			List<Arrival> liveData = e.Arrivals;
			var scheduled = db.getNextArrivals (myStop.stopId, aheadTime, DateTime.Now);
			//go through scheduled list and keep buses (by full name) that are not in live
			foreach (Arrival sAr in scheduled) {
				bool contains = false;
				foreach (Arrival lAr in liveData) {
					if (lAr.Destination == sAr.Destination) {//found bus with matching nmae
						contains = true;
						break;
					}
				}
				if (!contains && sAr.Time < DateTime.Now.Add (aheadTime))//also check whether within timeframe
					masterList.Add (sAr);//add scheduled arrival since live data not tracking that bus
			}
			//merge with livedata that is within after and ahead window
			masterList.AddRange (liveData.Where ((x) => (x.Time > afterTime && x.Time < DateTime.Now.Add (aheadTime))));
			masterList.Sort ();

			results = masterList;
			notify ();
		}

		private void notify ()
		{
			ArrivalEvent e = new ArrivalEvent (results, myStop);
			if (ArrivalsUpdated != null)
				ArrivalsUpdated (this, e);
		}

		/// <summary>
		/// Gets the results.
		/// </summary>
		/// <value>
		/// Null if results are still processing, empty if there are just no arrivals.
		/// </value>
		public List<Arrival> Results {
			get {
				return results;
			}
		}
	}
}

