using System;
using System.Collections.Generic;
using MonoTouch.CoreLocation;

namespace RITNow
{
	public class LocationComparer : IComparer<CLLocation>
	{
		CLLocation origin;
		/// <summary>
		/// Compares distances from origin
		/// </summary>
		/// <param name='origin'>
		/// Origin.
		/// </param>
		public LocationComparer (CLLocation origin)
		{
			this.origin = origin;
		}
		
		int IComparer<CLLocation>.Compare(CLLocation a, CLLocation b){
			return (int)(origin.DistanceFrom(a)-origin.DistanceFrom(b));
		}
	}
}

