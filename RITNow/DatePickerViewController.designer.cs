// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace RITNow
{
	[Register ("DatePickerViewController")]
	partial class DatePickerViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIDatePicker datePicker { get; set; }

		[Action ("doneTouched:")]
		partial void doneTouched (MonoTouch.Foundation.NSObject sender);

		[Action ("nowTouched:")]
		partial void nowTouched (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (datePicker != null) {
				datePicker.Dispose ();
				datePicker = null;
			}
		}
	}
}
