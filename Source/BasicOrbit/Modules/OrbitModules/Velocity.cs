#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit Velocity - Readout module for surface velocity
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
	public class Velocity : BasicModule
	{
		public Velocity(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showVelocity = IsVisible;
			BasicSettings.Instance.showVelocityAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";
			
			return result(FlightGlobals.ActiveVessel.horizontalSrfSpeed, FlightGlobals.ActiveVessel.verticalSpeed);
		}

		private string result(double h, double v)
		{
			return string.Format("Hor: {0} - Vert: {1}", h.Speed(1, 1), v.Speed(1, 1));
		}
	}
}