#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit OrbitAltitude - Readout module for altitude at current position
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
	public class OrbitAltitude : BasicModule
	{
		public OrbitAltitude(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showOrbitAltitude = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showOrbitAltitudeAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.altitude);
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Distance(0));
		}
	}
}