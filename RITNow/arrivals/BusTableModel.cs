using System;
using MonoTouch.CoreLocation;
using System.Linq;
using MonoTouch.Foundation;
using System.Collections.Generic;

namespace RITNow
{
	/// <summary>
	/// The logic for gathering the bus data for a single stop for display including location and refreshing.
	/// The view need only worry about the results of Arrivals and BusStop properties.
	/// A layer above ArrivalMonitor that fits the data in a format better for display
	/// </summary>
	public class BusTableModel
	{
		//TODO: User entered time and location

		private ArrivalMonitor monitor;
		private CLLocationManager locationManger;

		/// <summary>
		/// Backing field. Do not set directly, as the fetchers need to be changed.
		/// </summary>
		private BusStop? _myStop;
		const int INTERVAL = 15;

		//combine arrivals by route for better display
		private List<List<Arrival>> cachedBundles;

		/// <summary>
		/// Backing field. Use property which updates monitor.
		/// Null if live mode.
		/// </summary>
		private DateTime? _futureTime;

		public event ArrivalObserver ArrivalsUpdated;
		public delegate void ArrivalObserver (BusTableModel source); 


		/// <summary>
		/// Will follow user preference
		/// If they prefer current location, it will display the default stop, and update as location changes
		/// If they do not, it will display the default stop
		/// </summary>
		public BusTableModel () : this(BusDB_GTFS_SQL.Instance.getStopInfo(UserPreferences.DefaultStopId))
		{
			NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"NSUserDefaultsDidChangeNotification", handleSettingsChanged);
			initLocation ();
		}

		/// <summary>
		/// Create a model which only displays the given stop and doesn't touch preferences or use location
		/// </summary>
		/// <param name='stop'>
		/// A valid stop to display
		/// </param>
		public BusTableModel (BusStop? stop)
		{

			MonoTouch.Foundation.NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (INTERVAL), Refresh);
			DisplayedStop = stop;
		}

		/// <summary>
		/// Follows user preferences for stop
		/// Displays arrivals starting at futureTime instead of now
		/// </summary>
		/// <param name='futureTime'>
		/// The time to find arrivals after
		/// </param>
		public BusTableModel (DateTime futureTime) : this()
		{
			//not the most efficient way since calling the other contructer makes a monitor that is immediately thrown out
			FutureTime = futureTime;
		}

	
		/// <summary>
		/// Each list contains a list of arrivals sharing a destination
		/// </summary>
		public List<List<Arrival>> ArrivalBundles {
			get {
				return cachedBundles;
			}
		}

		public bool DataAvailable {
			get {
				return DisplayedStop.HasValue && ArrivalBundles != null;
			}
		}
		/// <summary>
		/// Get or set the stop to watch. Updates monitor when setting.
		/// </summary>
		/// <value>
		/// My stop.
		/// </value>
		public BusStop? DisplayedStop {
			get {
				return _myStop;
			}
			set {
				BusStop? oldStop = _myStop;
				if (value != null && (!oldStop.HasValue || ((BusStop)oldStop).stopId != ((BusStop)value).stopId)) {//changed stops, not null
					_myStop = value;
					reinitializeArrivalMonitor ();
					Refresh ();
				} //same stop, different object


			}
		}

		public DateTime? FutureTime {
			get {
				return _futureTime;
			}
			set {
				_futureTime = value;
				reinitializeArrivalMonitor ();
			}
		}

		private void reinitializeArrivalMonitor ()
		{
			if (DisplayedStop == null)
				monitor = null;
			else {
				if (FutureTime == null)//live mode
					monitor = new ArrivalMonitor ((BusStop)DisplayedStop);
				else//time travel mode
					monitor = new ArrivalMonitor ((BusStop)DisplayedStop, FutureTime.Value);
				monitor.ArrivalsUpdated+=arrivalsUpdated;
			}
			Refresh();
		}

		private void arrivalsUpdated (ArrivalMonitor source, ArrivalEvent e)
		{
			if (monitor != null && source == monitor && e.Error == false) {
//				cachedArrivals = e.Arrivals.ToArray ();
//
//				//combine arrivals by route for better display
//				cachedBundles=new List<ArrivalBundle>();
//				ArrivalBundle lastBundle=null;
//				if (cachedArrivals.Length>0){
//					foreach (Arrival a in cachedArrivals){
//						if (lastBundle==null||lastBundle.Bus!=a.Bus){
//							if (lastBundle!=null)
//								cachedBundles.Add (lastBundle);
//							lastBundle=new ArrivalBundle(a);
//						} else
//							lastBundle.Times.Add (a.Time);
//					}
//					cachedBundles.Add (lastBundle);
//				}
				Dictionary<string, int> destinationToListPos = new Dictionary<string, int>();
				List<List<Arrival>> bundles = new List<List<Arrival>>();
				//should be sorted by the monitor
				foreach (Arrival a in e.Arrivals){
					if (destinationToListPos.ContainsKey(a.Destination)){
						//use the dictionary to find which bundle to add to
						bundles[destinationToListPos[a.Destination]].Add (a);
					}
					else{
						//create a new list for this dest
						List<Arrival> newList = new List<Arrival>();
						newList.Add(a);

						//add it to the list of lists
						bundles.Add(newList);

						//make note of the index so we can retrieve it to add more
						destinationToListPos.Add(a.Destination, bundles.Count-1);
					}
				}
				cachedBundles=bundles;
			}
			ArrivalsUpdated (this);
		}
		/// <summary>
		/// There was a significant location change
		/// </summary>
		/// <param name='newLoc'>
		/// New location.
		/// </param>
		private void  locationChanged (CLLocation newLoc)
		{
			if (!UserPreferences.PreferCurrentLocation)
				return;
//			var allStops = BusDB_GTFS_SQL.Instance.getAllBusStops ();
//			allStops = allStops.OrderBy (stop => stop.location, new LocationComparer (newLoc));
//			var closest = ((BusStop)allStops.First ());
//			DisplayedStop = closest as BusStop?;
//			UserPreferences.DefaultStopId = closest.stopId;//update the default stop (careful about loops!)
			UserPreferences.DefaultStopId = BusDB_GTFS_SQL.Instance.getClosestStop(newLoc).Value.stopId;
		}

		private void initLocation ()
		{
			//location stuff
			locationManger = new CLLocationManager ();
			locationManger.DistanceFilter = 15;//must move 10 meters between updates (seems reasonable)
			locationManger.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters * 3;//30 meter accuracy
			locationManger.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
				locationChanged (e.NewLocation);
			};
			if (CLLocationManager.LocationServicesEnabled && UserPreferences.PreferCurrentLocation)
				locationManger.StartUpdatingLocation ();
		}

		public void Refresh ()
		{
			if (monitor != null)
				monitor.RefreshResults ();

		}

		private void handleSettingsChanged (NSNotification not)
		{
			if (UserPreferences.PreferCurrentLocation)
				locationManger.StartUpdatingLocation ();//handled when location updates
			else {
				locationManger.StopUpdatingLocation ();
				int potentialNewStopId = UserPreferences.DefaultStopId;
				if (DisplayedStop.HasValue && potentialNewStopId != DisplayedStop.Value.stopId)//it's different from what we have
					DisplayedStop = BusDB_GTFS_SQL.Instance.getStopInfo (UserPreferences.DefaultStopId);//avoids unessesary updates
			}

		}
	}
}

