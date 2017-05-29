#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit AngleToPrograde - Readout module for angle to prograde of current body
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

namespace BasicOrbit.Modules.TargetModules
{
	public class AngleToPrograde : BasicModule
	{
		public AngleToPrograde(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showAngleToPrograde = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showAngleToProgradeAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			if (BasicTargetting.ActiveBody == null)
				return "---";

			double UT = Planetarium.GetUniversalTime();

			if (BasicTargetting.ActiveBody.referenceBody == null || BasicTargetting.ActiveBody.referenceBody == BasicTargetting.ActiveBody)
				return "---";

			Vector3d ship = BasicTargetting.TrueShipOrbit.getRelativePositionAtUT(UT);
			ship.z = 0;

			Vector3d body = BasicTargetting.TrueShipOrbit.referenceBody.orbit.getOrbitalVelocityAtUT(UT);
			body.z = 0;
			
			double angle = Vector3d.Angle(body, ship);

            Vector3d r = QuaternionD.AngleAxis(90.0, Vector3d.forward) * ship;

            if (Vector3d.Angle(r, body) > 90.0)
                angle = 360.0 - angle;

			return result(angle);
		}

		private string result(double d)
		{
			return string.Format("{0:F2}°", d);
		}
	}
}
