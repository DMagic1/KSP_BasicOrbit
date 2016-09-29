using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class ArgOfPeriapsis : BasicModule
	{
		public ArgOfPeriapsis(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showAoPe = IsVisible;
			BasicSettings.Instance.showAoPeAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.orbit.argumentOfPeriapsis);
		}

		private string result(double d)
		{
			return string.Format("{0:N2}°", d);
		}
	}
}
