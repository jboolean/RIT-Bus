using System;
using System.Collections.Generic;

namespace RITNow
{
	/// <summary>
	/// Gets live and database data on a schedule or when prompted
	/// Reconciled differences, and notifies when there are changes
	/// Acts as a kind of facade to the schedule subsystem
	/// </summary>
	public class ArrivalEvent : EventArgs
	{
		public ArrivalEvent (List<Arrival> arrivals, BusStop stop)
		{
			this.Arrivals = arrivals;
			this.Stop = stop;
			Error=false;
		}

		/// <summary>
		/// Something went wrong. Data are invalid.
		/// </summary>
		public bool Error;
		
		public List<Arrival> Arrivals;
		public BusStop Stop;
	}

}

