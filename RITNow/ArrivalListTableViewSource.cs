using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
namespace RITNow
{
	public class ArrivalListTableViewSource : UITableViewSource
	{
		public BusTableModel model;
		UITableView tableView;
		public ArrivalListTableViewSource (UITableView tableView)
		{
			this.tableView=tableView;
			model=new BusTableModel();
			model.ArrivalsUpdated+=arrivalsUpdated;
		}

		private void arrivalsUpdated (BusTableModel source)
		{
			InvokeOnMainThread(delegate {
				tableView.ReloadData();
				//tableView.ReloadSections(MonoTouch.Foundation.NSIndexSet.FromIndex(0), UITableViewRowAnimation.Automatic);
			});

		}
		//table view stuff
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			/*	int row = indexPath.Row;
			if (row == getFavoriteStopsToDisplay().Length) {
				UITableViewCell moreCell = tableView.DequeueReusableCell ("moreCell");
				moreCell.TextLabel.Text = "All buses";
				return moreCell;
			} else {//bus cell
				string stopName = getFavoriteStopsToDisplay()[row];
				if (getRunsToDisplay().ContainsKey(stopName)){
					BusCellView busCell = tableView.DequeueReusableCell("busCell") as BusCellView;
					busCell.Reinitalize(getRunsToDisplay()[stopName].ToArray(), getOrigin(), stopName);
					return busCell;
				}
				else {
					UITableViewCell noCell = tableView.DequeueReusableCell("noBusesCell");
					noCell.TextLabel.Text="No buses to "+stopName;
					return noCell;
				}
			}

*/
			if (!model.DataAvailable) {
				UITableViewCell noCell = tableView.DequeueReusableCell ("infoCell");
				noCell.TextLabel.Text = "Loading...";
				return noCell;
			}else if (model.ArrivalBundles.Count==0) {
				UITableViewCell noCell = tableView.DequeueReusableCell ("infoCell");
				noCell.TextLabel.Text = "No Service";
				return noCell;
			} else {
				BusCellView2 busCell = tableView.DequeueReusableCell("busCell2") as BusCellView2;
				busCell.DisplayedArrivals = model.ArrivalBundles[indexPath.Row];
				return busCell;
			}
		}


		public override int NumberOfSections (UITableView tableView)
		{
			return 1;
		}
		public override int RowsInSection (UITableView tableview, int section)
		{
			switch (section) {
			default:
				if (model.DataAvailable&&model.ArrivalBundles.Count>0)
					return model.ArrivalBundles.Count;
				else
					return 1;//1 for the loading info cell
			}
		}
		/*public override string TitleForHeader (UITableView tableView, int section)
		{
			switch (section) {
			case 0:
				if (model.DataAvailable)
					return "Departures from "+((BusStop)model.DisplayedStop).name;
				else
					return "Departures";
			default:
				throw new Exception("Header count is off");
			}
		}*/
		public override float GetHeightForRow (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			if (model.DataAvailable&&indexPath.Row<model.ArrivalBundles.Count)
				return 98f;
			else
				return 44f;
		}
		public void Refresh(){
			model.Refresh();
		}



	}
}

