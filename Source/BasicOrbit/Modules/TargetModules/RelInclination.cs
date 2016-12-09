#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit RelInclination - Readout module for target relative inclination
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
	public class RelInclination : BasicModule
	{
		public RelInclination(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showRelInclination = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showRelInclinationAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			double angle = Vector3d.Angle(BasicTargetting.ShipOrbit.GetOrbitNormal(), BasicTargetting.TargetOrbit.GetOrbitNormal());

			return result(angle, getNextNode());
		}

		private double getNextNode()
		{
			Vector3d nodeAsc = Vector3d.Cross(BasicTargetting.TargetOrbit.GetOrbitNormal(), BasicTargetting.ShipOrbit.GetOrbitNormal());

			Vector3d nodeDsc = Vector3d.Cross(BasicTargetting.ShipOrbit.GetOrbitNormal(), BasicTargetting.TargetOrbit.GetOrbitNormal());

			double anomalyAsc = BasicTargetting.ShipOrbit.GetTrueAnomalyOfZupVector(nodeAsc) * Mathf.Rad2Deg;

			double anomalyDsc = BasicTargetting.ShipOrbit.GetTrueAnomalyOfZupVector(nodeDsc) * Mathf.Rad2Deg;

			double timeAsc = BasicTargetting.ShipOrbit.GetDTforTrueAnomaly(anomalyAsc * Mathf.Deg2Rad, BasicTargetting.ShipOrbit.period);

			double timeDsc = BasicTargetting.ShipOrbit.GetDTforTrueAnomaly(anomalyDsc * Mathf.Deg2Rad, BasicTargetting.ShipOrbit.period);

			if (timeAsc < 0 && !double.IsNaN(BasicTargetting.ShipOrbit.period) && !double.IsInfinity(BasicTargetting.ShipOrbit.period))
				timeAsc += BasicTargetting.ShipOrbit.period;

			if (timeDsc < 0 && !double.IsNaN(BasicTargetting.ShipOrbit.period) && !double.IsInfinity(BasicTargetting.ShipOrbit.period))
				timeDsc += BasicTargetting.ShipOrbit.period;

			return timeAsc < timeDsc ? timeAsc : timeDsc;
		}

		private string result(double d, double t)
		{
			return string.Format("{0:F3}°; node in {1}", d, t.Time(2));
		}
	}
}
