using System;

namespace RITNow
{
	public interface ArrivalObserver
	{
		void ArrivalsUpdated(BusStop stop, List<Arrival> newData);
	}
}

