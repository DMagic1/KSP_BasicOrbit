#region License
/*
 * Basic Orbit
 * 
 * BasicOrbiting - Static class for calculating terrain height information
 * 
 * Copyright (C) 2016 DMagic
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by 
 * the Free Software Foundation, either version 3 of the License, or 
 * (at your option) any later version. 
 * 
 * This program is distributed in the hope that it will be useful, 
 * but WITHOUT ANY WARRANTY; without even the implied warranty of 
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
 * GNU General Public License for more details. 
 * 
 * You should have received a copy of the GNU General Public License 
 * along with this program.  If not, see <http://www.gnu.org/licenses/>. 
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public static class BasicOrbiting
	{
		private static bool _updated;
		private static double _terrainHeight;
		private static double _radarAltitude;
		private static List<Part> _partList;
		private static List<KeyValuePair<Part, double>> _partsList;

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
			else if (FlightGlobals.ActiveVessel.LandedOrSplashed)
			{
				_terrainHeight = FlightGlobals.ActiveVessel.terrainAltitude;
				_radarAltitude = 0;
			}
			else
			{
				if (!FlightGlobals.ActiveVessel.isEVA && FlightGlobals.ActiveVessel.altitude - FlightGlobals.ActiveVessel.pqsAltitude.doubleMax(0) < 2400)
				{
					_terrainHeight = FlightGlobals.ActiveVessel.terrainAltitude;
					_radarAltitude = heightToLand();
				}
				else
				{
					_terrainHeight = FlightGlobals.ActiveVessel.terrainAltitude;
					_radarAltitude = FlightGlobals.ActiveVessel.radarAltitude;
				}
			}

			_updated = true;
		}

		//The following methods are adapted from SirDiazo's Landing Height Mod, also release under the GPLv3:
		//https://github.com/SirDiazo/LandingHeight
		//They are used to calculate the distance from the lowest point on a vessel to the surface		
		public static double heightToLand()
		{
			double landHeight = 0;
			bool firstRay = true;

			if (_partList == null)
				_partList = new List<Part>();

			if (_partsList == null)
				_partsList = new List<KeyValuePair<Part, double>>();

			_partList.Clear();

			if (FlightGlobals.ActiveVessel.Parts.Count < 50)
			{
				for (int i = FlightGlobals.ActiveVessel.Parts.Count - 1; i >= 0; i--)
				{
					Part p = FlightGlobals.ActiveVessel.Parts[i];

					if (p == null)
						continue;

					if (p.collider == null)
						continue;

                    if (p.collider.enabled)
						_partList.Add(p);
				}
			}
			else
			{
				_partsList.Clear();

                for (int i = FlightGlobals.ActiveVessel.Parts.Count - 1; i >= 0; i--)
				{
					Part p = FlightGlobals.ActiveVessel.Parts[i];

					_partsList.Add(new KeyValuePair<Part, double>(p, Vector3.SqrMagnitude(p.transform.position - FlightGlobals.ActiveVessel.mainBody.position)));
				}

				_partsList.Sort((a, b) => a.Value.CompareTo(b.Value));
                
                for (int i = 0; i < 30; i++)
				{
					KeyValuePair<Part, double> pp = _partsList[i];

                    if (pp.Key == null)
                        continue;

                    if (pp.Key.collider == null)
                        continue;

					if (pp.Key.collider.enabled)
						_partList.Add(pp.Key);
				}
			}

            for (int i = _partList.Count - 1; i >= 0; i--)
			{
				Part p = _partList[i];

				try
				{
					Vector3 partEdge = p.collider.ClosestPointOnBounds(FlightGlobals.currentMainBody.position);
					RaycastHit pHit;
					Vector3 rayDir = (FlightGlobals.currentMainBody.position - partEdge).normalized;
					Ray pRayDown = new Ray(partEdge, rayDir);

                    if (Physics.Raycast(pRayDown, out pHit, (float)(FlightGlobals.ActiveVessel.mainBody.Radius + FlightGlobals.ActiveVessel.altitude), 32769))
                    {
                        if (pHit.collider.gameObject.layer == 15 || (pHit.collider.gameObject.layer == 0 && pHit.collider.gameObject.GetComponentInParent<Part>().vessel != FlightGlobals.ActiveVessel))
                        {
                            float hitDist = pHit.distance;

                            if (FlightGlobals.ActiveVessel.mainBody.ocean)
                            {
                                if (FlightGlobals.ActiveVessel.PQSAltitude() < 0)
                                    hitDist = hitDist + (float)FlightGlobals.ActiveVessel.PQSAltitude();
                            }

                            if (firstRay)
                            {
                                landHeight = hitDist;

                                firstRay = false;
                            }
                            else if (hitDist < landHeight)
                                landHeight = hitDist;
                        }

                    }
                    else if (!firstRay)
                    {
                        if (FlightGlobals.ActiveVessel.pqsAltitude < 0)
                            landHeight = Math.Min(landHeight, FlightGlobals.ActiveVessel.altitude);
                        else
                            landHeight = Math.Min(landHeight, FlightGlobals.ActiveVessel.altitude - FlightGlobals.ActiveVessel.pqsAltitude);
                        firstRay = false;
                    }
				}
				catch
				{
					landHeight = FlightGlobals.ActiveVessel.radarAltitude;
					firstRay = false;
				}
			}

			if (!firstRay && landHeight == 0)
			{
				landHeight = FlightGlobals.ActiveVessel.radarAltitude;
				firstRay = false;
			}

			return landHeight;
		}

	}
}
