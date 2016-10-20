#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit ManClosestRelVel - Readout module for relative velocity at closest approach after maneuver node
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

namespace BasicOrbit.Modules.ManeuverModules
{
	public class ManClosestRelVel : BasicModule
	{
		public ManClosestRelVel(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showManeuverClosestVel = IsVisible;
			BasicSettings.Instance.showManeuverClosestVelAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicManeuvering.Updated)
				return "---";

			if (BasicManeuvering.VesselIntersect)
				return result(BasicManeuvering.ClosestRelVelocity);

			return "---";
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Speed());
		}
	}
}
