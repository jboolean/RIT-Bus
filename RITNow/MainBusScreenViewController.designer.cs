// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace RITNow
{
	[Register ("MainBusScreenViewController")]
	partial class MainBusScreenViewController
	{
		[Outlet]
		MonoTouch.UIKit.UITableView busTable { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel currentLocationLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UILabel currentDateLabel { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView timeButtonView { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIView stopButtonView { get; set; }

		[Action ("refreshTouched:")]
		partial void refreshTouched (MonoTouch.Foundation.NSObject sender);

		[Action ("changeDateTouched:")]
		partial void changeDateTouched (MonoTouch.Foundation.NSObject sender);

		[Action ("nowButton_TouchDown:")]
		partial void nowButton_TouchDown (MonoTouch.Foundation.NSObject sender);

		[Action ("nowButton_TouchUpOutside:")]
		partial void nowButton_TouchUpOutside (MonoTouch.Foundation.NSObject sender);

		[Action ("stopButton_TouchDown:")]
		partial void stopButton_TouchDown (MonoTouch.Foundation.NSObject sender);

		[Action ("stopButton_TouchUpOutside:")]
		partial void stopButton_TouchUpOutside (MonoTouch.Foundation.NSObject sender);

		[Action ("stopButton_TouchUpInside:")]
		partial void stopButton_TouchUpInside (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (busTable != null) {
				busTable.Dispose ();
				busTable = null;
			}

			if (currentLocationLabel != null) {
				currentLocationLabel.Dispose ();
				currentLocationLabel = null;
			}

			if (currentDateLabel != null) {
				currentDateLabel.Dispose ();
				currentDateLabel = null;
			}

			if (timeButtonView != null) {
				timeButtonView.Dispose ();
				timeButtonView = null;
			}

			if (stopButtonView != null) {
				stopButtonView.Dispose ();
				stopButtonView = null;
			}
		}
	}
}
