using System;
using System.Collections.Generic;

namespace RITNow
{
	/// <summary>
	/// A list of times for a specific route at a specific stop. 
	/// Exactly like arrival, but allows for multiple times
	/// </summary>
	public class ArrivalBundle
	{
		public BusStop Stop;
		public BusRoute Bus;
		public string CommonDestination;
		public List<DateTime> Times;
		public bool Live;

		public ArrivalBundle (BusStop stop, BusRoute bus, List<DateTime> times, bool live=false)
		{
			this.Stop = stop;
			this.Bus = bus;
			this.Times = times;
			this.Live = live;
			this.CommonDestination=bus.Long_Name;
		}
		public ArrivalBundle (BusStop stop, BusRoute bus, bool live)
		{
			this.Stop = stop;
			this.Bus = bus;
			this.Live = live;
			this.CommonDestination=bus.Long_Name;
		}
		public ArrivalBundle (Arrival firstArrival)
		{
			this.Bus=firstArrival.Bus;
			this.Stop=firstArrival.Stop;
			this.Live=firstArrival.Live;
			Times=new List<DateTime>();
			Times.Add(firstArrival.Time);
			this.CommonDestination=firstArrival.Destination;
		}
		public ArrivalBundle ()
		{
		}
		

		public int CompareTo (ArrivalBundle b)
		{
			if (this.Times.Count==0)
				return 1;
			if (b.Times.Count==0)
				return -1;
			return this.Times[0].CompareTo(b.Times[0]);
		}
	}
}

