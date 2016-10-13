#region License
/*
 * Basic Orbit
 * 
 * BasicTargetting - Static class for calculating target encouter information
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

namespace BasicOrbit.Modules.TargetModules
{
	public static class BasicTargetting
	{
		private static double _closestDist;
		private static double _closestTime;
		private static double _closestRelVel;
		private static bool _update;
		private static bool _isVessel;
		private static bool _isCelestial;
		private static bool _vesselIntersect;
		private static bool _bodyIntersect;

		private static List<OrbitTargeter.Marker> _markers;

		private static bool _updated;

		private static Orbit _shipOrbit;
		private static Orbit _trueShipOrbit;
		private static Orbit _targetOrbit;
		private static CelestialBody _targetBody;

		public static bool Updated
		{
			get { return _updated; }
		}

		public static bool IsVessel
		{
			get { return _isVessel; }
		}

		public static bool IsCelestial
		{
			get { return _isCelestial; }
		}

		public static bool VesselIntersect
		{
			get { return _vesselIntersect; }
		}

		public static bool BodyIntersect
		{
			get { return _bodyIntersect; }
		}

		public static Orbit ShipOrbit
		{
			get { return _shipOrbit; }
		}

		public static Orbit TrueShipOrbit
		{
			get { return _trueShipOrbit; }
		}

		public static Orbit TargetOrbit
		{
			get { return _targetOrbit; }
		}

		public static double ClosestDistance
		{
			get { return _closestDist; }
		}

		public static double ClosestTime
		{
			get { return _closestTime; }
		}

		public static double ClosestRelVelocity
		{
			get { return _closestRelVel; }
		}

		public static bool UpdateOn
		{
			get { return _update; }
			set { _update = value; }
		}

		public static bool TargetValid()
		{
			if (FlightGlobals.ActiveVessel == null ||
				FlightGlobals.ActiveVessel.targetObject == null ||
				FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null ||
				FlightGlobals.ActiveVessel.targetObject.GetOrbit().referenceBody == null ||
				FlightGlobals.ActiveVessel.orbit == null ||
				FlightGlobals.ActiveVessel.orbit.referenceBody == null)
			{
				_targetBody = null;
				_vesselIntersect = false;
				_bodyIntersect = false;
				_updated = false;
				return false;
			}

			if (FlightGlobals.ActiveVessel.targetObject.GetVessel() == null)
			{
				if (FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null)
				{
					_targetBody = null;
					_isCelestial = false;
					_isVessel = false;
				}
				else if (FlightGlobals.ActiveVessel.targetObject.GetOrbitDriver().celestialBody != null)
				{
					_targetBody = FlightGlobals.ActiveVessel.targetObject.GetOrbitDriver().celestialBody;
					_isCelestial = true;
					_isVessel = false;
				}
				else
				{
					_targetBody = null;
					_isCelestial = false;
					_isVessel = false;
				}
			}
			else
			{
				_targetBody = null;
				_isCelestial = false;
				_isVessel = true;
			}

			return true;
		}

		public static void Update()
		{
			_update = false;

			if (FlightGlobals.ActiveVessel == null ||
				FlightGlobals.ActiveVessel.targetObject == null ||
				FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null ||
				FlightGlobals.ActiveVessel.targetObject.GetOrbit().referenceBody == null ||
				FlightGlobals.ActiveVessel.orbit == null ||
				FlightGlobals.ActiveVessel.orbit.referenceBody == null)
			{
				_updated = false;
				return;
			}

			_updated = true;

			_targetOrbit = FlightGlobals.ActiveVessel.targetObject.GetOrbit();

			Orbit active = FlightGlobals.ActiveVessel.orbit;

			_trueShipOrbit = active;

			if (active.referenceBody == Planetarium.fetch.Sun || active.referenceBody == FlightGlobals.ActiveVessel.targetObject.GetOrbit().referenceBody)
				_shipOrbit = active;
			else
				_shipOrbit = FlightGlobals.ActiveVessel.orbit.referenceBody.orbit;

			Vessel.Situations sit = FlightGlobals.ActiveVessel.situation;

			if ((sit |= Vessel.Situations.LANDED | Vessel.Situations.SPLASHED | Vessel.Situations.PRELAUNCH) == 0)
			{
				_vesselIntersect = false;
				_bodyIntersect = false;
			}
			else
			{
				if (!_isVessel && !_isCelestial)
				{
					_vesselIntersect = false;
					_bodyIntersect = false;
				}
				else
				{
					OrbitTargeter oTargeter = FlightGlobals.ActiveVessel.orbitTargeter;

					if (oTargeter == null)
					{
						_vesselIntersect = false;
						_bodyIntersect = false;
					}
					else if (!MapView.MapIsEnabled)
					{
						_vesselIntersect = false;
						_bodyIntersect = false;
					}
					else
					{
						if (_markers == null || _markers.Count <= 0)
							_markers = BasicOrbitReflection.GetOrbitMarkers(oTargeter);

						if (_markers == null || _markers.Count <= 0)
						{
							_vesselIntersect = false;
							_bodyIntersect = false;
						}
						else if (_isVessel)
						{
							_bodyIntersect = false;

							OrbitTargeter.ISectMarker _intersectOne = null;
							OrbitTargeter.ISectMarker _intersectTwo = null;

							for (int i = _markers.Count - 1; i >= 0; i--)
							{
								OrbitTargeter.Marker m = _markers[i];

								if (m == null)
									continue;

								if (!(m is OrbitTargeter.ISectMarker))
									continue;

								int num = ((OrbitTargeter.ISectMarker)m).num;

								if (num == 1)
									_intersectOne = m as OrbitTargeter.ISectMarker;
								else if (num == 2)
									_intersectTwo = m as OrbitTargeter.ISectMarker;
							}

							OrbitTargeter.ISectMarker _closestIntersect = null;

							if (_intersectOne != null && _intersectTwo != null)
								_closestIntersect = _intersectOne.separation > _intersectTwo.separation ? _intersectTwo : _intersectOne;
							else if (_intersectOne != null)
								_closestIntersect = _intersectOne;
							else if (_intersectTwo != null)
								_closestIntersect = _intersectTwo;
							else
								_closestIntersect = null;

							if (_closestIntersect == null)
								_vesselIntersect = false;
							else
							{
								_vesselIntersect = true;
								_closestDist = _closestIntersect.separation * 1000;
								_closestRelVel = _closestIntersect.relSpeed;
								_closestTime = _closestIntersect.UT;
							}
						}
						else if (_isCelestial)
						{
							_vesselIntersect = false;

							PatchedConicSolver solver = FlightGlobals.ActiveVessel.patchedConicSolver;

							if (solver != null)
							{
								double Pe = double.MaxValue;

								if (solver.flightPlan.Count > 0)
								{
									for (int i = solver.flightPlan.Count - 1; i >= 0; i--)
									{
										Orbit o = solver.flightPlan[i];

										if (o == null)
											continue;

										if (o.timeToPe > 0 && o.referenceBody == _targetBody && (o.StartUT + o.timeToPe < o.EndUT) && o.PeR > o.referenceBody.Radius && o.PeR < o.referenceBody.sphereOfInfluence)
										{
											if (Pe > o.PeA)
											{
												Pe = o.PeA;
												_closestTime = o.StartUT + o.timeToPe;
											}
										}
									}
								}
								else
								{
									for (int i = solver.patches.Count - 1; i >= 0; i--)
									{
										Orbit o = solver.patches[i];

										if (o == null)
											continue;

										if (o.timeToPe > 0 && o.referenceBody == _targetBody && (o.StartUT + o.timeToPe < o.EndUT) && o.PeR > o.referenceBody.Radius && o.PeR < o.referenceBody.sphereOfInfluence)
										{
											if (Pe > o.PeA)
											{
												Pe = o.PeA;
												_closestTime = o.StartUT + o.timeToPe;
											}
										}
									}
								}

								if (Pe < double.MaxValue - 1000)
								{
									_closestDist = Pe;
									_bodyIntersect = true;
								}
								else
									_bodyIntersect = false;
							}

							if (!_bodyIntersect)
							{
								OrbitTargeter.ClApprMarker _approach = null;

								for (int i = _markers.Count - 1; i >= 0; i--)
								{
									OrbitTargeter.Marker m = _markers[i];

									if (m == null)
										continue;

									if (!(m is OrbitTargeter.ClApprMarker))
										continue;

									_approach = m as OrbitTargeter.ClApprMarker;
								}

								if (_approach == null)
									_bodyIntersect = false;
								else
								{
									_bodyIntersect = true;
									_closestDist = _approach.separation * 1000;
									_closestTime = (_approach.dT * -1) + Planetarium.GetUniversalTime();
								}
							}
						}

					}
				}
			}
		}

	}
}
