sing System;
using System.Linq;
using System.Collections.Generic;

namespace RITNow
{
	public class BusScheduleDB
	{
		List<BusRun> db;
		public BusScheduleDB ()
		{
			db = new List<BusRun>();

			//fake stuff
			BusRun run = new BusRun(new Dictionary<string, TimeSpan>(), true, true, true);
			db.Add (run);
			run.populateWithFakeData();
		}
		public List<BusRun> getRuns (string origin, string destination)
		{
			return db.FindAll(run=>run.Stops.ContainsKey(origin)&&run.Stops.ContainsKey(destination));
		} 
	}
}

