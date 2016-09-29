using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class RelVelocityAtClosest : BasicModule
	{
		public RelVelocityAtClosest(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showClosestApproachVelocity = IsVisible;
			BasicSettings.Instance.showClosestApproachVelocityAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			if (BasicTargetting.IsVessel)
			{
				if (BasicTargetting.ClosestIntersect == null)
					return "---";

				return result(BasicTargetting.ClosestIntersect.relSpeed);
			}
			else
				return "---";
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Speed());
		}
	}
}
