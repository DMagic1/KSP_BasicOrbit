using System;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public static class BasicOrbiting
	{
		private static bool _updated;
		private static double _terrainHeight;
		private static double _radarAltitude;

		public static bool Updated
		{
			get { return _updated; }
		}

		public static double TerrainHeight
		{
			get { return _terrainHeight; }
		}

		public static double RadarAltitude
		{
			get { return _radarAltitude; }
		}

		public static void Update()
		{
			_updated = false;

			if (FlightGlobals.ActiveVessel == null ||
				FlightGlobals.currentMainBody == null)
				return;

			if (FlightGlobals.currentMainBody.pqsController == null)
			{
				_terrainHeight = 0;
				_radarAltitude = FlightGlobals.ActiveVessel.altitude;
			}
			else if (FlightGlobals.ActiveVessel.heightFromTerrain < 0)
			{
				_terrainHeight = FlightGlobals.ActiveVessel.PQSAltitude();
				_radarAltitude = FlightGlobals.ActiveVessel.altitude - _terrainHeight;
			}
			else
			{
				_terrainHeight = FlightGlobals.ActiveVessel.terrainAltitude;
				_radarAltitude = FlightGlobals.ActiveVessel.radarAltitude;
			}

			_updated = true;
		}

	}
}
