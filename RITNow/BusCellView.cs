// This file has been autogenerated from parsing an Objective-C header file added in Xcode.
using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using System.Timers;

namespace RITNow
{
	public partial class BusCellView : UITableViewCell
	{
		private Arrival _myArrival;

		public BusCellView (IntPtr handle) : base (handle)
		{
			NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (5), repaint);
		}

		/// <summary>
		/// Change the run and destination that this cell displayson. must be in run
		/// </param>
		public Arrival Arrival {
			get {
				return _myArrival;
			}
			set {
				_myArrival = value;
				repaint ();
			}
		}
		/// <summary>
		/// Gets the cell's mode (one bus or a repeating route)
		/// </summary>


		private void repaint ()
		{
			this.busNameLbl.Text = _myArrival.Bus.Long_Name;
			this.clockLbl.Text = ((int)_myArrival.Time.Subtract (DateTime.Now).TotalMinutes).ToString ();
			this.liveLbl.Text = (_myArrival.Live) ? "GPS Tracked" : "";
		}


	}
}
