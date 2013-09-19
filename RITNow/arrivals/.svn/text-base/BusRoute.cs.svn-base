using System;

namespace RITNow
{
	public struct BusRoute : IEquatable<BusRoute>, IComparable<BusRoute>
	{
		public string Short_Name;
		public string Long_Name;
		//public BusStop[] Stops;
		public string FullName {
			get {
				return "Route "+Short_Name+" to "+Long_Name;
			}
		}


		/*
		 * Names vs numbers vs fullnames:
		 * A full name is how it appears on WMB e.g. "Route 28 to RIT Night Shuttle"
		 * A number is something like 28, or sometimes bus numbers have letters like Z8
		 * A name is usually the "Destination" but RTS uses it to distinuish amount RIT buses
		 * 	e.g. "RIT Night Shuttle" or "Colony/Perkins" 
		 */

		public BusRoute (string number, string name) 
		{
			this.Short_Name = number;
			this.Long_Name = name;
		}
		public BusRoute (string fullname)
		{
			fullname=fullname.Trim();
			if (fullname.StartsWith ("Route ")) {
				string numStart = fullname.Substring (fullname.IndexOf (' ')+1);
				this.Short_Name =  numStart.Substring(0, numStart.IndexOf(' '));
				this.Long_Name = fullname.Substring (fullname.IndexOf (" to ")+4);
			} else {
				this.Short_Name="";
				this.Long_Name="";
			}
		}

		public bool Equals (BusRoute b)
		{
			return this.FullName.Equals(b.FullName);
		}
		public static bool operator ==(BusRoute a, BusRoute b){
			return a.Equals(b);
		}
		public static bool operator != (BusRoute a, BusRoute b)
		{
			return !(a == b);
		}
		public int CompareTo(BusRoute b){
			return this.Long_Name.CompareTo(b.Long_Name);
		}
		
		
	}
}

