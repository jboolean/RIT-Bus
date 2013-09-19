using System;
using MonoTouch.CoreLocation;

namespace RITNow
{
	public struct BusStop : IEquatable<BusStop>
	{
		public string name;
		public int stopId;
		public CLLocation location;
		public BusStop (string name, int stopId, CLLocation location)
		{
			this.name = name;
			this.stopId = stopId;
			this.location = location;
		}
		public override string ToString ()
		{
			return name+" - "+stopId;
		}
		///Equal if stop ids are
		public bool Equals(BusStop o){
			return this.stopId==o.stopId;
		}	
		public static bool operator ==(BusStop a, BusStop b){
			return a.Equals(b);
		}
		public static bool operator !=(BusStop a, BusStop b){
			return !(a==b);
		}

	}
}

