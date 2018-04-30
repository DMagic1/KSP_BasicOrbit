#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit Maneuver AngleToPrograde - Readout module for angle to prograde of current body at maneuver node position
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

namespace BasicOrbit.Modules.ManeuverModules
{
	public class ManAngleToPro : BasicModule
	{
		public ManAngleToPro(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showManeuverAngleToPrograde = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showManeuverAngleToProgradeAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicManeuvering.Updated)
				return "---";

			if (BasicManeuvering.Node == null)
				return "---";

			double UT = BasicManeuvering.Node.UT;

			Orbit patch = BasicManeuvering.Node.patch;

			Vector3d ship = patch.getRelativePositionAtUT(UT);
			ship.z = 0;

			Vector3d body = patch.referenceBody.orbit.getOrbitalVelocityAtUT(UT);
			body.z = 0;

			double angle = Vector3d.Angle(body, ship);

			Vector3d r = QuaternionD.AngleAxis(90.0, Vector3d.forward) * ship;

			if (Vector3d.Angle(r, body) > 90.0)
				angle = 360.0 - angle;

			return result(angle);
		}

		private string result(double d)
		{
			return string.Format("{0}°", d.ToString("F2"));
		}
	}
}
