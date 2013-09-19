using System;
using System.Linq;

namespace RITNow
{
	/// <summary>
	/// A time of day without the date
	/// </summary>
	public class BusSchedule
	{
		private TimeSpan timeOfDay;

		public BusSchedule (Int32 hour, Int32 minute, Int32 seconds)
		{
			timeOfDay = new TimeSpan(hour, minute, seconds);

		}

		static DayOfWeek[] weekdays = {DayOfWeek.Friday, DayOfWeek.Monday, DayOfWeek.Thursday, DayOfWeek.Tuesday, DayOfWeek.Wednesday};
		static DayOfWeek[] weekends = {DayOfWeek.Saturday, DayOfWeek.Sunday};
		public DateTime NextOccurance {
			get {
				DateTime nextTime = DateTime.Today.Add (timeOfDay);
				while (DateTime.Now>nextTime||(weekdays.Contains(nextTime.DayOfWeek)&&!OperatesWeekdays)||(weekends.Contains(nextTime.DayOfWeek)&&!OperatesWeekends)||(IsAHoliday(nextTime.DayOfWeek)&&!OperatesHolidays))
					nextTime.AddDays(1);
				return nextTime;
			}
		}
		public TimeSpan TimeUntilNextOccurance {
			get {
				return this.NextOccurance.Subtract (DateTime.Now);
			}
		}
		public static bool IsAHoliday(DateTime day){
			//TODO
			return false;
		}

	}
}

