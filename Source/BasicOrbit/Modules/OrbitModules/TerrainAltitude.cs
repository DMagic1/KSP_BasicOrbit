using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class TerrainAltitude : BasicModule
	{
		public TerrainAltitude(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showTerrain = IsVisible;
			BasicSettings.Instance.showTerrainAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.terrainAltitude <= 0)
				return "---";

			return result(FlightGlobals.ActiveVessel.terrainAltitude);
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Distance());
		}
	}
}
