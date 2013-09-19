// WARNING
//
// This file has been generated automatically by MonoDevelop to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;

namespace RITNow
{
	[Register ("DatePickerView")]
	partial class DatePickerView
	{
		[Outlet]
		MonoTouch.UIKit.UIDatePicker datePicker { get; set; }

		[Action ("nowClicked:")]
		partial void nowClicked (MonoTouch.Foundation.NSObject sender);

		[Action ("doneClicked:")]
		partial void doneClicked (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (datePicker != null) {
				datePicker.Dispose ();
				datePicker = null;
			}
		}
	}
}
