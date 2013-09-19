using System;
using System.Collections.Generic;
using System.Linq;

namespace RITNow
{
	/// <summary>
	/// Represents one trip of a Bus. Two locaitons and the time it hits each. 
	/// Also contains operating days. Used mainly to retrieve the time until arrival.
	/// May have a strategy to pull live data in the future.
	/// </summary>
	/// 
	public class BusTrip
	{

		private Destination origin, destination;
		public bool OperatesWeekdays;
		public bool OperatesWeekends;
		public bool OperatesHolidays;
		public TimeSpan clockTimeOrigin, clockTimeDestination;
		public BusRoute myRoute;
		public BusTrip (BusRoute myRoute, Destination origin, Destination destination, bool weekdays, TimeSpan originArrival, TimeSpan destArrival, bool weekends, bool holidays)
		{
			this.origin=origin;
			this.clockTimeOrigin=originArrival;
			this.clockTimeDestination=destArrival;
			this.destination=destination;
			OperatesHolidays=holidays;
			OperatesWeekdays= weekdays;
			OperatesWeekends=weekdays;

		}

		static DayOfWeek[] weekdays = {DayOfWeek.Friday, DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Tuesday, DayOfWeek.Wednesday};
		static DayOfWeek[] weekends = {DayOfWeek.Saturday, DayOfWeek.Sunday};
		public TimeSpan getTimeUntilArrival ()
		{
			DateTime nextTime = DateTime.Today.Add (this.clockTimeOrigin);
			//add a day until it runs that day
		
			while (DateTime.Now>nextTime||(weekdays.Contains(nextTime.DayOfWeek)&&!OperatesWeekdays)||(weekends.Contains(nextTime.DayOfWeek)&&!OperatesWeekends)||(BusDB.IsAHoliday(nextTime)&&!OperatesHolidays))
				nextTime=nextTime.AddDays(1);
			return nextTime.Subtract(DateTime.Now);
		}


	}
}

