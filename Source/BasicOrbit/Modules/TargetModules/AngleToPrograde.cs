using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class AngleToPrograde : BasicModule
	{
		public AngleToPrograde(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showAngleToPrograde = IsVisible;
			BasicSettings.Instance.showAngleToProgradeAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (!BasicTargetting.Updated)
				return "---";

			double UT = Planetarium.GetUniversalTime();

			CelestialBody b = FlightGlobals.ActiveVessel.mainBody;

			if (b.referenceBody == null || b.referenceBody == b)
				return "---";

			Vector3d ship = BasicTargetting.TrueShipOrbit.getRelativePositionAtUT(UT);
			ship.z = 0;

			Vector3d body = BasicTargetting.TrueShipOrbit.referenceBody.orbit.getOrbitalVelocityAtUT(UT);
			body.z = 0;
			
			double angle = Vector3d.Angle(body, ship);

            Vector3d r = QuaternionD.AngleAxis(90.0, Vector3d.forward) * ship;

            if (Vector3d.Angle(r, body) > 90.0)
                angle = 360.0 - angle;

			return result(angle);
		}

		private string result(double d)
		{
			return string.Format("{0:F2}°", d);
		}
	}
}
