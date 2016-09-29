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
				return "---";

			//return result(BasicTargetting.ClosestDistance, BasicTargetting.ClosestTime - Planetarium.GetUniversalTime());
			
			if (BasicTargetting.IsVessel)
			{
				if (BasicTargetting.ClosestIntersect == null)
					return "---";

				double distance = BasicTargetting.ClosestIntersect.separation * 1000;
				double time = BasicTargetting.ClosestIntersect.UT - Planetarium.GetUniversalTime();

				return result(distance, time);
			}
			else if (BasicTargetting.IsCelestial)
			{
				if (BasicTargetting.ApproachMarker == null)
					return "---";

				double distance = BasicTargetting.ApproachMarker.separation * 1000;
				double time = -1 * BasicTargetting.ApproachMarker.dT;

				return result(distance, time);
			}
			else
				return "---";
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.CloseDistance(), KSPUtil.PrintTime(t, 3, false));
		}
	}
}
