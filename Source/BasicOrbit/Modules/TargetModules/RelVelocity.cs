using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class RelVelocity : BasicModule
	{
		public RelVelocity(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showRelVelocity = IsVisible;
			BasicSettings.Instance.showRelVelocityAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "";

			return result(FlightGlobals.ship_tgtSpeed);
		}

		private string result(double d)
		{
			return string.Format("{0:F3}°", d.Speed());
		}
	}
}
