using System;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

namespace RITNow
{
	public class LiveArrivalFetcher
	{
		/// <summary>
		/// Pause or unpause fetching of data. Please disable when not being used.
		/// </summary>
		public bool Enabled = true;
		private bool processing = false;
		private BusStop myStop;
		private List<Arrival> results = null;

		public event ArrivalObserver ArrivalsUpdated;
		public delegate void ArrivalObserver (LiveArrivalFetcher m,ArrivalEvent e); 

		/// <summary>
		/// Creates an object which continuously checks the WMB website at interval and reports to observer
		/// </summary>
		/// <param name='stopId'>
		/// Stop identifier.
		/// </param>
		/// <param name='observer'>
		/// Will be notified when there may be changes
		/// </param>
		/// <param name='interval'>
		/// Interval frequency in seconds or -1 to just run once
		/// </param>
		public LiveArrivalFetcher (BusStop stop)
		{
			this.myStop = stop; 
		}

		const string RTS_URL ="http://wmb.rgrta.com/mob/SearchBy.aspx";
		//const string RTS_URL = "http://julianboilen.com/rtsTest.html";
		const string VIEWSTATE_FILE = "viewstate.txt";

		public List<Arrival> doFetchNow ()
		{	
			if (!Enabled)
				return null;
			processing = true;
			MonoTouch.UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = true;
			List<Arrival> outList = new List<Arrival> ();

			try {
				string postData = "ctl00%24mainPanel%24searchbyStop%24txtStop=" + myStop.stopId + "&ctl00%24mainPanel%24btnGetRealtimeSchedule=GO";
				postData += "&__VIEWSTATE=" + File.ReadAllText (VIEWSTATE_FILE);

				byte[] data = (new System.Text.ASCIIEncoding ()).GetBytes (postData);

				WebRequest request = WebRequest.Create (RTS_URL);
				request.Method = "POST";
				request.ContentLength = data.Length;
				request.ContentType = "application/x-www-form-urlencoded";
				Stream dataStream = request.GetRequestStream ();
				dataStream.Write (data, 0, data.Length);
				dataStream.Close ();

				WebResponse response = request.GetResponse ();
				//string responseData = (new StreamReader (response.GetResponseStream ())).ReadToEnd ();

				HtmlDocument doc = new HtmlDocument ();
				doc.Load (response.GetResponseStream ());
				var table = doc.GetElementbyId ("ctl00_mainPanel_gvSearchResult");
				foreach (var row in table.Elements("tr")) {
					var tds = row.Elements ("td");
					if (row.Elements ("td").Count () == 2) {//crude test for header and general correctness
						var dest = tds.First ();
						if (dest.InnerText.Trim () == "No Service")
							break;
						var time = tds.Last ();
						Arrival a = new Arrival (myStop, new BusRoute (dest.InnerText), parseRtsTime (time.InnerText), true);
						outList.Add (a);
					}
				}
				response.Close ();

				results = outList;
			} catch (Exception e) {
				//something went wrong, fail gracefully
				Console.WriteLine ("Fetch error: " + e.Message);
				notify (true);
				return null;
			} finally {
				MonoTouch.UIKit.UIApplication.SharedApplication.NetworkActivityIndicatorVisible = false;
				processing = false;
			}
			notify (false);
			return outList;
		}

		public static DateTime parseRtsTime (string rtsTime)
		{
			//TODO: Eastern time
			rtsTime = rtsTime.Trim ();
			DateTime tmp;
			if (rtsTime.Contains ("min")) {
				int min = int.Parse (rtsTime.Substring (0, rtsTime.IndexOf (" ")));
				//add one since 1 min means between 1 and 2 min etc.
				return DateTime.Now.AddMinutes ((double)(++min));
			} else  if (rtsTime == "Due") {
				return DateTime.Now.AddMinutes (1.0);
			} else if (DateTime.TryParse (rtsTime, out tmp)) {
				DateTime time = DateTime.Parse (rtsTime);
				if (time < DateTime.Now)
					return time.AddDays (1.0);
				else
					return time;
			} else 
				throw new FormatException (rtsTime + " does not comform to RTS time format.");
		}

		/// <summary>
		/// Does the fetch as a background task. Reports back to the observer.
		/// The observer should get the results from the Results property.
		/// </summary>
		public void DoFetchBackground ()
		{
			if (!processing)
				Task.Factory.StartNew (() => {
					this.doFetchNow ();}
				);

		}

		private void notify (bool error=false)
		{
			ArrivalEvent e = new ArrivalEvent (results, myStop);
			e.Error = error;
			if (ArrivalsUpdated != null)
				ArrivalsUpdated (this, e);
		}

		/// <summary>
		/// Gets the results from the last fetch
		/// </summary>
		/// <value>
		/// The results.
		/// </value>
		public List<Arrival> Results {
			get {
				return results;
			}
		}

		/*public static bool Reachable {
			get {
				//TODO: Implement
				return true;
			}
		}*/

	}

}

