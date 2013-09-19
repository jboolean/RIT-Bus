using System;

namespace RITNow
{
	public struct Arrival: IComparable<Arrival>
	{
		public BusStop Stop;
		public BusRoute Bus;
		public DateTime Time;
		public string Destination;
		public bool Live;
		public Arrival (BusStop stop, BusRoute bus, DateTime time, bool live=false)
		{
			this.Stop = stop;
			this.Bus = bus;
			this.Time = time;
			this.Live=live;
			Destination=bus.Long_Name;
		}
		public Arrival (BusStop stop, BusRoute bus, DateTime time, string destination, bool live=false)
		{
			this.Stop = stop;
			this.Bus = bus;
			this.Time = time;
			this.Destination = destination;
			this.Live = live;
		}
		
		public override string ToString(){
			return Bus.FullName+" arriving at " + Stop.ToString() + " at "+Time.ToShortTimeString();
		}
		public int CompareTo(Arrival b){
			return this.Time.CompareTo(b.Time);
			/*long ticks= this.Time.Subtract(b.Time).Ticks;
			if (ticks==0)
				return 0;
			else if (ticks<0)
				return -1;
			else 
				return 1;*/
		}

		
	}
}

