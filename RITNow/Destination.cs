using System;
using MonoTouch.CoreLocation;

namespace RITNow
{
	public struct Destination
	{
		public string name;
		public int stopId;
		public CLLocationCoordinate2D location;
		public Destination (string name, int stopId, CLLocationCoordinate2D location)
		{
			this.name = name;
			this.stopId = stopId;
			this.location = location;
		}
		
	}
}

