using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class Periapsis : BasicModule
	{
		public Periapsis(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showPeriapsis = IsVisible;
			BasicSettings.Instance.showPeriapsisAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.orbit.PeA, FlightGlobals.ActiveVessel.orbit.timeToPe);
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.Distance(), KSPUtil.PrintTime(t, 3, false));
		}
	}
}