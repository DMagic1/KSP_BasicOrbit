using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class RelInclination : BasicModule
	{
		public RelInclination(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showRelInclination = IsVisible;
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

			if (timeAsc < 0)
				timeAsc += BasicTargetting.ShipOrbit.period;

			if (timeDsc < 0)
				timeDsc += BasicTargetting.ShipOrbit.period;

			return timeAsc < timeDsc ? timeAsc : timeDsc;
		}

		private string result(double d, double t)
		{
			return string.Format("{0:F3}°; node in {1}", d, KSPUtil.PrintTime(t, 2, false));
		}
	}
}
