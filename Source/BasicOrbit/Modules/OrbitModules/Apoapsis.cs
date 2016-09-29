using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class Apoapsis : BasicModule
	{
		public Apoapsis(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showApoapsis = IsVisible;
			BasicSettings.Instance.showApoapsisAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.orbit.ApA, FlightGlobals.ActiveVessel.orbit.timeToAp);
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.Distance(), KSPUtil.PrintTime(t, 3, false));
		}
	}
}
