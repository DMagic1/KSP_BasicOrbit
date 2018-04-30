#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit VesselName - Readout module for the vessel name
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
	public class VesselName : BasicModule
	{
		public VesselName(string t)
			: base(t)
		{
            _cutoffText = true;
		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showVesselName = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showVesselNameAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";
            
			return FlightGlobals.ActiveVessel.vesselName;
		}
	}
}
