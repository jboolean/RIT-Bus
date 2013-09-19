// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace RITNow
{
	[Register ("BusStopMapViewController")]
	partial class BusStopMapViewController
	{
		[Outlet]
		MonoTouch.MapKit.MKMapView MapView { get; set; }

		[Action ("BackTouched:")]
		partial void BackTouched (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (MapView != null) {
				MapView.Dispose ();
				MapView = null;
			}
		}
	}
}
