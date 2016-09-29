using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class Inclination : BasicModule
	{
		public Inclination(string t)
			: base(t)
		{
			
		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showInclination = IsVisible;
			BasicSettings.Instance.showInclinationAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.orbit.inclination);
		}

		private string result(double d)
		{
			return string.Format("{0:F3}°", d);
		}
	}
}
