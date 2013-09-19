
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace RITNow
{
	/// <summary>
	/// A date picker that allows choosing of a future date or now.
	/// Designed to be displayed in an actionsheet by calling ShowInActionSheet.
	/// Get results from the event DatePicked
	/// </summary>
	public partial class DatePickerView : UIViewController
	{
		/// <summary>
		/// Occurs when the view is closed.
		/// </summary>
		public event PickerObserver DatePicked;

		UIActionSheet pickerSheet;

		public delegate void PickerObserver (bool nowPicked,DateTime date); 


		/// <summary>
		/// Used to pass parameter from constructor to viewdidload
		/// </summary>
		private DateTime defaultDate;

		public DatePickerView () : this(DateTime.Now)
		{
		}

		public DatePickerView (DateTime defaultDate) : base ("DatePickerView", null)
		{
			this.defaultDate = defaultDate;
		}

		/// <summary>
		/// Shows me in an action sheet
		/// </summary>
		public void ShowInActionSheet (UIView container)
		{
			pickerSheet = new UIActionSheet ();
			pickerSheet.ShowInView (container);
			int height = 260;
			var frame = new System.Drawing.RectangleF (0, container.Frame.Height-height, container.Frame.Width, height);
			pickerSheet.Frame = frame;
			
			this.View.Frame = new System.Drawing.RectangleF (0, 0, 320, height);
			pickerSheet.AddSubview (this.View);


		}

		partial void nowClicked (NSObject sender)
		{
			DismisActionSheet ();
			if (DatePicked != null)
				DatePicked (true, TimeZoneInfo.ConvertTimeFromUtc(datePicker.Date, TimeZoneInfo.Local));
		}

		partial void doneClicked (NSObject sender)
		{
			DismisActionSheet ();
			if (DatePicked != null)
				DatePicked (false, TimeZoneInfo.ConvertTimeFromUtc(datePicker.Date, TimeZoneInfo.Local));

		}

		/// <summary>
		/// Dismises the action sheet if shown
		/// </summary>
		public void DismisActionSheet ()
		{
			if (pickerSheet != null)
				pickerSheet.DismissWithClickedButtonIndex (0, true);
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}
		
		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			datePicker.TimeZone=NSTimeZone.LocalTimeZone;
			datePicker.Date = defaultDate;
			datePicker.MinimumDate=DateTime.Now.AddMinutes(30);

			
			// Perform any additional setup after loading the view, typically from a nib.
		}
		
		public override void ViewDidUnload ()
		{
			base.ViewDidUnload ();
			
			// Clear any references to subviews of the main view in order to
			// allow the Garbage Collector to collect them sooner.
			//
			// e.g. myOutlet.Dispose (); myOutlet = null;
			
			ReleaseDesignerOutlets ();
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			// Return true for supported orientations
			return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
		}
	}
}

