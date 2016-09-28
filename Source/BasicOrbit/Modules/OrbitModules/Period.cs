using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class Period : BasicModule
	{
		public Period(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showPeriod = IsVisible;
			BasicSettings.Instance.showPeriodAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.orbit.period);
		}

		private string result(double t)
		{
			return KSPUtil.dateTimeFormatter.PrintTime(t, 3, false);
		}
	}
}
