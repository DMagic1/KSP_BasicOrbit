#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit Maneuver Apoapsis - Readout module for orbit Apoapsis after maneuver node
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
	public class ManApoapsis : BasicModule
	{
		public ManApoapsis(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showManeuverApoapsis = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showManeuverApoapsisAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicManeuvering.Updated)
				return "---";

			if (BasicManeuvering.Node == null)
				return "---";

			if (BasicManeuvering.Node.nextPatch == null)
				return "---";

			if (BasicManeuvering.Node.nextPatch.eccentricity >= 1)
				return "---";

			return result(BasicManeuvering.Node.nextPatch.ApA, (BasicManeuvering.Node.nextPatch.StartUT + BasicManeuvering.Node.nextPatch.timeToAp) - Planetarium.GetUniversalTime());
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.Distance(), t.Time(3));
		}
	}
}
