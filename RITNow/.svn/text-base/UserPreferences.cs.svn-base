using System;
using MonoTouch.Foundation;

namespace RITNow
{
	/// <summary>
	/// A proxy to the underlying preference system. 
	/// Watch for changes using NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"NSUserDefaultsDidChangeNotification", handler)
	/// </summary>
	public static class UserPreferences
	{

		public static bool PreferCurrentLocation {
			get {
				return NSUserDefaults.StandardUserDefaults.BoolForKey ("PreferCurrentLocation");
			}
			set {
				if (value != PreferCurrentLocation) {
					NSUserDefaults.StandardUserDefaults.SetBool (value, "PreferCurrentLocation");
					NSUserDefaults.StandardUserDefaults.Synchronize ();
				}
			}
		}

		public static int DefaultStopId {
			get {
				return NSUserDefaults.StandardUserDefaults.IntForKey ("DefaultStopId");
			}
			set {
				if (value != DefaultStopId) {
					NSUserDefaults.StandardUserDefaults.SetInt (value, "DefaultStopId");
					NSUserDefaults.StandardUserDefaults.Synchronize ();
				}
			}
		}
	}
}

