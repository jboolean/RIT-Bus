
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
	public partial class DatePickerViewController : UIViewController
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

		public DatePickerViewController () : this(DateTime.Now)
		{
		}

		public DatePickerViewController (DateTime defaultDate) : base ("DatePickerViewController", null)
		{
			this.defaultDate = defaultDate;
		}

		/// <summary>
		/// Shows me in an action sheet
		/// </summary>
		public void ShowInActionSheet (UIView container)
		{
			Console.WriteLine ("VIEW LOADED: " + this.IsViewLoaded);
			pickerSheet = new UIActionSheet ();
			pickerSheet.ShowInView (container);
			var frame = new System.Drawing.RectangleF (0, 260, container.Frame.Width, 260);
			pickerSheet.Frame = frame;

		}

		partial void nowTouched (NSObject sender)
		{
			DismisActionSheet ();
			PickerObserver (true, datePicker.Date);
		}

		partial void doneTouched (NSObject sender)
		{
			DismisActionSheet ();
			PickerObserver (false, datePicker.Date);
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
			datePicker.Date = defaultDate;

			this.View.Frame = new System.Drawing.RectangleF (0, 10, 320, 210);
			pickerSheet.AddSubview (this.View);
			
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

