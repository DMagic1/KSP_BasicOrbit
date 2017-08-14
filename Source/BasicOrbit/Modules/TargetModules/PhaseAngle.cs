#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit PhaseAngle - Readout module for phase angle of target body
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
	public class PhaseAngle : BasicModule
	{
		public PhaseAngle(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showPhaseAngle = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showPhaseAngleAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			if (BasicTargetting.ShipOrbit == null || BasicTargetting.TargetOrbit == null)
				return "---";

			Vector3d exclusion = Vector3d.Exclude(BasicTargetting.ShipOrbit.GetOrbitNormal(), BasicTargetting.TargetOrbit.pos);

			double angle = Vector3d.Angle(exclusion, BasicTargetting.ShipOrbit.pos);

			Vector3d rotate = QuaternionD.AngleAxis(90, Vector3d.forward) * BasicTargetting.ShipOrbit.pos;

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
