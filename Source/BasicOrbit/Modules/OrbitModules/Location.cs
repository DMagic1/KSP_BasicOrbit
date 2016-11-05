#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit Location - Readout module for location coordinates
 * 
 * Copyright (C) 2016 DMagic
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by 
 * the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. 
 * 
 * This program is distributed in the hope that it will be useful, 
 * but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
 * GNU General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License 
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 * 
 * 
 */
#endregion

namespace BasicOrbit.Modules.OrbitModules
{
	public class Location : BasicModule
	{
		public Location(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showLocation = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showLocationAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			return result(((FlightGlobals.ActiveVessel.latitude + 180 + 90) % 180 - 90), ((FlightGlobals.ActiveVessel.longitude + 360 + 180) % 360 - 180));
		}

		private string result(double lat, double lon)
		{
			return string.Format("{0} {1}", lat.DMS('S', 'N'), lon.DMS('W', 'E'));
		}
	}
}
