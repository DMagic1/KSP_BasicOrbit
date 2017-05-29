#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit Maneuver PhaseAngle - Readout module for phase angle of target body at maneuver node position
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

using UnityEngine;
using BasicOrbit.Modules.TargetModules;

namespace BasicOrbit.Modules.ManeuverModules
{
	public class ManPhaseAngle : BasicModule
	{
		public ManPhaseAngle(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showManeuverPhaseAngle = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showManeuverPhaseAngleAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicManeuvering.Updated)
				return "---";

			if (BasicManeuvering.Node == null)
				return "---";

			if (BasicManeuvering.PhasingNodePatch == null || BasicManeuvering.TargetPhasingOrbit == null)
				return "---";

			double UT = BasicManeuvering.Node.UT;

			Vector3d exclusion = Vector3d.Exclude(BasicManeuvering.PhasingNodePatch.GetOrbitNormal(), BasicManeuvering.TargetPhasingOrbit.getRelativePositionAtUT(UT));

			double angle = Vector3d.Angle(exclusion, BasicManeuvering.PhasingNodePatch.getRelativePositionAtUT(UT));

			Vector3d rotate = QuaternionD.AngleAxis(90, Vector3d.forward) * BasicManeuvering.PhasingNodePatch.getRelativePositionAtUT(UT);

			if (Vector3d.Angle(rotate, exclusion) > 90)
				angle = 360 - angle;

			return result(angle);
		}

		private string result(double d)
		{
			return string.Format("{0:F2}°", d);
		}
	}
}
