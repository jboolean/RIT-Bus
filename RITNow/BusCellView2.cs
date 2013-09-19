// This file has been autogenerated from parsing an Objective-C header file added in Xcode.
using System;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using System.Timers;
using System.Collections.Generic;

namespace RITNow
{
	public partial class BusCellView2 : UITableViewCell
	{
		private List<Arrival> _myArrivals;

		public BusCellView2 (IntPtr handle) : base (handle)
		{
			NSTimer.CreateRepeatingScheduledTimer (TimeSpan.FromSeconds (5), repaint);
		}

		/// <summary>
		/// A SORTED list of Arrivals with a COMMON DESTINATION. 
		/// I know that's a lot of preprossessing - this is just a view!
		/// </param>
		public List<Arrival> DisplayedArrivals {
			get {
				return _myArrivals;
			}
			set {
				_myArrivals = value;
				repaint ();
			}
		}

		private void repaint ()
		{
			if (_myArrivals==null || _myArrivals.Count<1)
				return;
			//the big clock for the next arrival
			{
				this.destLabel.Text = _myArrivals[0].Destination;
				int minsUntil = ((int)_myArrivals[0].Time.Subtract (DateTime.Now).TotalMinutes);
				if (minsUntil < 1) {
					clockLabel.Text = "DUE";
					this.minutesLabel.Hidden = true;
				} else if (minsUntil < 60) {
					clockLabel.Text = minsUntil.ToString ();
					this.minutesLabel.Hidden = false;
					this.minutesLabel.Text = "minutes";
				} else {
					clockLabel.Text = _myArrivals[0].Time.ToString ("h:mm");
					this.minutesLabel.Hidden = false;
					this.minutesLabel.Text = _myArrivals[0].Time.ToString ("tt");
				}
			}

			this.signalImage.Hidden = !_myArrivals[0].Live;

			//the next times label
			{

				string nextText = "";
				//each arrival time, comma separated
				foreach (Arrival a in _myArrivals) {
					string thisTimeText;
					int minsUntil = ((int)a.Time.Subtract (DateTime.Now).TotalMinutes);
					if (minsUntil < 1) {
						thisTimeText = "Due";
					} else if (minsUntil < 60) {
						thisTimeText = minsUntil.ToString ()+"m";
					} else {
						thisTimeText = a.Time.ToString ("h:mm tt");
					}

					nextText+=((nextText!="")?", ":"")+thisTimeText;
				}
				nextLabel.Text=nextText;
			}
		}

		public override void LayoutSubviews ()
		{
			this.clockLabel.Font = UIFont.FromName ("Citaro Voor (Dubbele hoogte, breed)", 85);
			this.minutesLabel.Font = UIFont.FromName ("dotspecial", 8);
			applyGlow (this.clockLabel);
			applyGlow (this.minutesLabel);
		}

		private void applyGlow (UIView view)
		{
			view.Layer.ShadowColor = (new UIColor ((float)254 / 255, (float)99 / 255, (float)33 / 255, 1.0f)).CGColor;
			view.Layer.ShadowRadius = 3.0f;
			view.Layer.ShadowOpacity = 0.78f;
			view.Layer.ShadowOffset = new System.Drawing.SizeF (0.0f, 0.0f);
			view.Layer.MasksToBounds = false;
		}

	}
}
