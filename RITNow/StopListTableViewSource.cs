using System;
using MonoTouch.UIKit;
using MonoTouch.CoreLocation;
using System.Linq;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace RITNow
{
	public class StopListTableViewSource : UITableViewSource
	{
		private CLLocationManager locationManger;
		private SelectStopViewController myController;
		private BusStop[] allStops;
		private bool closestCalculated = false;
		private CLLocation myLocation;

		//before locating if user does not prefer current location it uses this one (gleason circle)
		private  CLLocation FALLBACK_LOCATION = new CLLocation(43.083384, -77.676045);
		const double METERS_PER_MILE = 1609;

		public StopListTableViewSource (SelectStopViewController myController)
		{
			//where to find stop about first
			CLLocation defaultLoc;
			if (UserPreferences.PreferCurrentLocation)
				defaultLoc = BusDB_GTFS_SQL.Instance.getStopInfo(UserPreferences.DefaultStopId).Value.location;
			else
				defaultLoc = FALLBACK_LOCATION;

			allStops = BusDB_GTFS_SQL.Instance.getAllBusStops (defaultLoc, 2*METERS_PER_MILE).ToArray ();
			this.myController = myController;
			initLocation ();

		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (isStopListSection (indexPath.Section)) {
				//the section with all the stops
				UITableViewCell stopCell;
				BusStop stop = allStops [indexPath.Row];

				//cells with distance
				if (closestCalculated && myLocation != null) {
					stopCell = tableView.DequeueReusableCell ("stopCellWithDistance");
					double distanceFrom = myLocation.Distancefrom (stop.location);
					distanceFrom *= 0.0006214;
					stopCell.DetailTextLabel.Text = "" + Math.Round (distanceFrom, 3) + " mi";

					//cell without distance
				} else {
					stopCell = tableView.DequeueReusableCell ("stopCellWithoutDistance");
				}

				//show checkmark on the bus cell if not on current location and this is the default stop
				if (UserPreferences.PreferCurrentLocation == false && stop.stopId == UserPreferences.DefaultStopId)
					stopCell.Accessory = UITableViewCellAccessory.Checkmark;
				else
					stopCell.Accessory = UITableViewCellAccessory.None;
				stopCell.TextLabel.Text = stop.name;

				return stopCell;
			} else {
				//the section to select the nearest
				UITableViewCell closestCell = tableView.DequeueReusableCell ("closestCell");

				//subtitle shows nearest stop
				closestCell.DetailTextLabel.Text = "Currently: " + allStops [0].name;

				//checkmark
				if (UserPreferences.PreferCurrentLocation)
					closestCell.Accessory = UITableViewCellAccessory.Checkmark;
				else
					closestCell.Accessory = UITableViewCellAccessory.None;
				return closestCell;
			}
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			//select a specific stop
			if (isStopListSection (indexPath.Section)) {
				BusStop stop = allStops [indexPath.Row];
				UserPreferences.DefaultStopId = stop.stopId;
				UserPreferences.PreferCurrentLocation = false;

			} else if (indexPath.Section == 0 && closestCalculated) {//follow location
				UserPreferences.PreferCurrentLocation=true;
			}
			myController.Close();

		}

		public override int NumberOfSections (UITableView tableView)
		{
			if (closestCalculated)
				return 2;
			else 
				return 1;
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			if (closestCalculated && section == 0 && allStops.Length>=1)
				return 1;
			else
				return allStops.Length;
		}
		public override string TitleForHeader (UITableView tableView, int section)
		{
			if (isStopListSection(section))
				return "Nearby Stops";
			return null;
		}

		/// <summary>
		/// Returns true if section is the section that lists stop, not the current location section
		/// </summary>
		/// <returns>
		/// The stop list section.
		/// </returns>
		/// <param name='section'>
		/// the section to test 0 or 1
		/// </param>
		private bool isStopListSection (int section)
		{
			return (!closestCalculated && section == 0) || (closestCalculated && section == 1);
		}

		private void  locationChanged (CLLocation newLoc)
		{

			//TODO: this in inefficient
			allStops = BusDB_GTFS_SQL.Instance.getAllBusStops(newLoc, 2*METERS_PER_MILE).OrderBy (stop => stop.location, new LocationComparer (newLoc)).ToArray ();
			closestCalculated = true;
			myLocation = newLoc;
			myController.Reload();
			locationManger.StopUpdatingLocation ();//only need one
		}

		private void initLocation ()
		{
			//location stuff
			locationManger = new CLLocationManager ();
			locationManger.DesiredAccuracy = CLLocation.AccuracyNearestTenMeters * 3;//30 meter accuracy
			locationManger.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
				locationChanged (e.NewLocation);
			};
			if (CLLocationManager.LocationServicesEnabled)
				locationManger.StartUpdatingLocation ();
		}


	}
}

