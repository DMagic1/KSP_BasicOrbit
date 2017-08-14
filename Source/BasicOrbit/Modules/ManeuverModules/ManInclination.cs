#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit Maneuver Inclination - Readout module for orbit inclination after manevuer node
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
	public class ManInclination : BasicModule
	{
		public ManInclination(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showManeuverInclination = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showManeuverInclinationAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicManeuvering.Updated)
				return "---";

			if (BasicManeuvering.Node == null)
				return "---";

			if (BasicManeuvering.Node.nextPatch == null)
				return "---";

			if (BasicManeuvering.TargetInclination)
			{
				double inc = Vector3d.Angle(BasicManeuvering.Node.nextPatch.GetOrbitNormal(), BasicManeuvering.TargetPhasingOrbit.GetOrbitNormal());

				return string.Format("{0} ({1})", result(BasicManeuvering.Node.nextPatch.inclination), targetResult(inc));
			}
			else
				return result(BasicManeuvering.Node.nextPatch.inclination);
		}

		private string result(double d)
		{
			return string.Format("{0:F3}°", d);
		}

		private string targetResult(double d)
		{
			return string.Format("Tgt Rel {0:F3}°", d);
		}
	}
}

