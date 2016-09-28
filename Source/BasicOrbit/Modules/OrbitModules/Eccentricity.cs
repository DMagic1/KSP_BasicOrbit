using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class Eccentricity : BasicModule
	{
		public Eccentricity(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showEccentricity = IsVisible;
			BasicSettings.Instance.showEccentricityAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(FlightGlobals.ActiveVessel.orbit.eccentricity);
		}

		private string result(double t)
		{
			return string.Format("{0:F4}", t);
		}
	}
}
