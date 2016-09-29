using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class SemiMajorAxis : BasicModule
	{
		public SemiMajorAxis(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showSMA = IsVisible;
			BasicSettings.Instance.showSMAAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.orbit.semiMajorAxis);
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Distance());
		}
	}
}
