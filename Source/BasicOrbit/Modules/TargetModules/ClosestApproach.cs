using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class ClosestApproach : BasicModule
	{
		public ClosestApproach(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showClosestApproach = IsVisible;
			BasicSettings.Instance.showClosestApproachAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "";

			Vector3d x = BasicTargetting.TargetOrbit.pos - BasicTargetting.ShipOrbit.pos;
			Vector3d v = BasicTargetting.TargetOrbit.vel - BasicTargetting.ShipOrbit.vel;
			double xv = Vector3d.Dot(x, v);
			
			double t = -xv / Vector3d.SqrMagnitude(v);

			return result(0, t);
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.Distance(), KSPUtil.dateTimeFormatter.PrintTime(t, 3, false));
		}
	}
}
