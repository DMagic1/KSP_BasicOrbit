#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit ManClosestApproach - Readout module for closest approach after maneuver
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
	public class ManClosestApproach : BasicModule
	{
		public ManClosestApproach(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showManeuverClosestApproach = IsVisible;
			BasicSettings.Instance.showManeuverClosestApproachAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicManeuvering.Updated)
				return "---";

			if (BasicManeuvering.VesselIntersect || BasicManeuvering.BodyIntersect)
				return result(BasicManeuvering.ClosestDistance, BasicManeuvering.ClosestTime - Planetarium.GetUniversalTime());

			return "---";
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.Distance(), t.Time(2));
		}
	}
}
