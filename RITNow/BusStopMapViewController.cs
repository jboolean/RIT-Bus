// This file has been autogenerated from parsing an Objective-C header file added in Xcode.
using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.MapKit;

namespace RITNow
{
	public partial class BusStopMapViewController : UIViewController
	{
		BusDB_GTFS_SQL db;

		public BusStopMapViewController (IntPtr handle) : base (handle)
		{
			db = BusDB_GTFS_SQL.Instance;
		}

		public override void ViewDidLoad ()
		{
			MapView.GetViewForAnnotation=GetViewForAnnotation;
			//Meters from center to display
			const double ZOOM = 300;
			//Center on user
			if (MapView.UserLocationVisible)
				MapView.Region = MKCoordinateRegion.FromDistance (MapView.UserLocation.Coordinate, ZOOM, ZOOM);
			//if not location available, use last bus stop if that works
			else {
				BusStop? lastStop = db.getStopInfo (UserPreferences.DefaultStopId);
				if (lastStop.HasValue)
					MapView.Region = MKCoordinateRegion.FromDistance (lastStop.Value.location.Coordinate, ZOOM, ZOOM);
			}
			addAnnotations();
		}

		private void addAnnotations ()
		{
			var allStops = db.getAllBusStops ();
			foreach (BusStop stop in allStops) {
				MapView.AddAnnotation (new StopAnnotation (stop));
			}
		}

		const string STOP_PIN_ID = "STOP_ANNOTATION_VIEW";
		MKAnnotationView GetViewForAnnotation (MKMapView mapView, NSObject annotation)
		{
			//Stop annotations
			if (annotation is StopAnnotation) {
				StopAnnotation stopAnn = annotation as StopAnnotation;

				//dequeue or create
				MKPinAnnotationView pin = mapView.DequeueReusableAnnotation(STOP_PIN_ID) as MKPinAnnotationView;
				if (pin==null){
					pin = new MKPinAnnotationView(stopAnn, STOP_PIN_ID);
					pin.CanShowCallout=true;
				}

				return pin;
			}//end is stop annotations
			return null;
		}

		partial void BackTouched (NSObject sender)
		{
			DismissModalViewControllerAnimated (true);
		}

		//An annotation which holds a stop and returns data (location, name) from it
		class StopAnnotation : MKAnnotation
		{
			public BusStop Stop;

			public StopAnnotation (BusStop stop)
			{
				this.Stop = stop;
			}
			public override string Title {
				get {
					return Stop.name;
				}
			}

			public override MonoTouch.CoreLocation.CLLocationCoordinate2D Coordinate {
				get {
					return Stop.location.Coordinate;
				}
				set {
				
				}
			}

			public override string Subtitle {
				get {
					return Stop.stopId.ToString();
				}
			}
		}//end StopAnnotation
	}
}
