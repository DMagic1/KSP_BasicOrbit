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
		private static bool _showAngle;
		private static bool _showPhasing;

		private static List<OrbitTargeter.Marker> _markers;

		private static int _patchLimit;
		private static bool _updated;

		private static Orbit _shipOrbit;
		private static Orbit _trueShipOrbit;
		private static Orbit _targetOrbit;
		private static CelestialBody _targetBody;
		private static Vessel _activeVessel;
		private static CelestialBody _activeBody;
		private static ITargetable _targetObject;
		private static Transform _targetTransform;
		private static Vector3 _vesselTargetDelta;

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

		public static bool ShowAngle
		{
			get { return _showAngle; }
		}

		public static bool ShowPhasing
		{
			get { return _showPhasing; }
		}

		public static int PatchLimit
		{
			set
			{
				if (value > 1)
					_patchLimit = value;
				else
					_patchLimit = 1;
			}
		}

		public static List<OrbitTargeter.Marker> Markers
		{
			get { return _markers; }
			set { _markers = value; }
		}

		public static CelestialBody TargetBody
		{
			get { return _targetBody; }
		}

		public static Vessel ActiveVessel
		{
			get { return _activeVessel; }
		}

		public static CelestialBody ActiveBody
		{
			get { return _activeBody; }
		}

		public static ITargetable TargetObject
		{
			get { return _targetObject; }
		}

		public static Transform TargetTransform
		{
			get { return _targetTransform; }
		}

		public static Vector3 VesselTargetDelta
		{
			get { return _vesselTargetDelta; }
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
				_activeVessel = null;
				_activeBody = null;
				_targetObject = null;
				_targetTransform = null;
				return false;
			}

			_activeVessel = FlightGlobals.ActiveVessel;
			_activeBody = FlightGlobals.currentMainBody;
			_targetObject = _activeVessel.targetObject;

			if (_targetObject.GetVessel() == null)
			{
				if (_targetObject.GetOrbit() == null)
				{
					_targetBody = null;
					_isCelestial = false;
					_isVessel = false;
				}
				else if (_targetObject.GetOrbitDriver().celestialBody != null)
				{
					_targetBody = _targetObject.GetOrbitDriver().celestialBody;
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
				_targetTransform = FlightGlobals.fetch.vesselTargetTransform;
				_vesselTargetDelta = FlightGlobals.fetch.vesselTargetDelta;
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

			Orbit active = _activeVessel.orbit;
			Orbit target = _targetObject.GetOrbit();

			_trueShipOrbit = active;

			_showAngle = false;
			_showPhasing = false;

			_shipOrbit = null;
			_targetOrbit = null;

			if (active.referenceBody == target.referenceBody)
			{
				_shipOrbit = active;
				_targetOrbit = target;
			}
			else
			{
				if (active.referenceBody == Planetarium.fetch.Sun)
				{
					_shipOrbit = active;
					_targetOrbit = target;
				}
				else
					_showAngle = true;

				_showPhasing = true;

				DrillDownOrbits(active, target);
			}

			Vessel.Situations sit = _activeVessel.situation;

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
					OrbitTargeter oTargeter = _activeVessel.orbitTargeter;
					PatchedConicSolver solver = _activeVessel.patchedConicSolver;

					if (oTargeter == null || solver == null)
					{
						_vesselIntersect = false;
						_bodyIntersect = false;
					}
					else if (!MapView.MapIsEnabled)
					{
						if (_isVessel)
						{
							_bodyIntersect = false;

							Vessel tgt = _targetObject.GetVessel();

							if (tgt == null || tgt.LandedOrSplashed)
							{
								_vesselIntersect = false;
								return;
							}

							Orbit _refPatch = null;

							if (solver.maneuverNodes.Count > 0)
								_refPatch = GetReferencePatch(oTargeter, solver, target.referenceBody, null, true);
							else
								_refPatch = BasicOrbitReflection.GetRefPatch(oTargeter);

							Orbit _tgtRefPatch = BasicOrbitReflection.GetTargetRefPatch(oTargeter);

							_vesselIntersect = GetClosestVessel(_refPatch, _tgtRefPatch);
						}
						else
						{
							_vesselIntersect = false;

							double Pe = GetLowestPeA(solver);

							if (Pe < BasicExtensions.AlmostMaxValue)
							{
								_closestDist = Pe;
								_bodyIntersect = true;
							}
							else
							{
								Orbit _refPatch = null;

								if (solver.maneuverNodes.Count > 0)
									_refPatch = GetReferencePatch(oTargeter, solver, _targetBody, target.referenceBody, false);
								else
									_refPatch = BasicOrbitReflection.GetRefPatch(oTargeter);

								if (_refPatch != null && _refPatch.closestTgtApprUT <= 0)
								{
									_bodyIntersect = false;
									return;
								}
								
								Orbit _tgtRefPatch = BasicOrbitReflection.GetTargetRefPatch(oTargeter);

								_bodyIntersect = GetClosestCelestial(_refPatch, _tgtRefPatch);
							}
						}
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

							if (solver.maneuverNodes.Count > 0)
							{
								Orbit _refPatch = GetReferencePatch(oTargeter, solver, target.referenceBody, null, true);
								Orbit _tgtRefPatch = BasicOrbitReflection.GetTargetRefPatch(oTargeter);

								_vesselIntersect = GetClosestVessel(_refPatch, _tgtRefPatch);
							}
							else
							{
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
						}
						else
						{
							_vesselIntersect = false;

							double Pe = GetLowestPeA(solver);

							if (Pe < BasicExtensions.AlmostMaxValue)
							{
								_closestDist = Pe;
								_bodyIntersect = true;
							}
							else
							{
								if (solver.maneuverNodes.Count > 0)
								{
									Orbit _refPatch = GetReferencePatch(oTargeter, solver, _targetBody, target.referenceBody, false);
									Orbit _tgtRefPatch = BasicOrbitReflection.GetTargetRefPatch(oTargeter);

									_bodyIntersect = GetClosestCelestial(_refPatch, _tgtRefPatch);
								}
								else
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

		private static void DrillDownOrbits(Orbit s, Orbit t)
		{
			bool sIsOrbitingPlanet = s.referenceBody.referenceBody.referenceBody == null ||
				s.referenceBody.referenceBody.referenceBody == s.referenceBody.referenceBody;
			bool tIsPlanet = t.referenceBody.referenceBody == null || t.referenceBody.referenceBody == t.referenceBody;

			if (tIsPlanet)
			{
				while (!(s.referenceBody.referenceBody == null || s.referenceBody.referenceBody == s.referenceBody))
					s = s.referenceBody.orbit;

				_shipOrbit = s;
				_targetOrbit = t;
			}
			else
			{
				CelestialBody targetParent = t.referenceBody;

				while (!(targetParent.referenceBody.referenceBody == null || targetParent.referenceBody.referenceBody == targetParent.referenceBody))
					targetParent = targetParent.referenceBody;

				if (sIsOrbitingPlanet)
				{
					if (s.referenceBody == targetParent)
					{
						_targetOrbit = t.referenceBody.orbit;
						_shipOrbit = s;
					}
					else
					{
						_targetOrbit = targetParent.orbit;
						_shipOrbit = s.referenceBody.orbit;
					}
				}
				else
				{
					CelestialBody shipParent = s.referenceBody;

					while (!(shipParent.referenceBody.referenceBody == null || shipParent.referenceBody.referenceBody == shipParent.referenceBody))
						shipParent = shipParent.referenceBody;

					if (shipParent == targetParent)
					{
						_shipOrbit = s.referenceBody.orbit;
						_targetOrbit = t;
					}
					else
					{
						_shipOrbit = shipParent.orbit;
						_targetOrbit = targetParent.orbit;
					}
				}
			}
		}

		private static Orbit GetSameReference(CelestialBody body, Orbit o)
		{
			if (o.referenceBody == null)
				return o;

			if (o.referenceBody == body)
				return o;

			if (o.referenceBody.orbit == null)
				return o;

			return GetSameReference(body, o.referenceBody.orbit);
		}

		private static double GetLowestPeA(PatchedConicSolver s)
		{
			double Pe = double.MaxValue;

			for (int i = _patchLimit - 1; i >= 0; i--)
			{
				Orbit o = s.patches[i];

				if (o == null)
					continue;

				if (!o.activePatch)
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

			return Pe;
		}

		private static bool GetClosestCelestial(Orbit refP, Orbit tgtP)
		{
			if (refP == null || tgtP == null)
				return false;

			Vector3d refClosest = refP.getRelativePositionAtUT(refP.closestTgtApprUT);
			Vector3d tgtClosest = tgtP.getRelativePositionAtUT(refP.closestTgtApprUT);

			_closestDist = (refClosest - tgtClosest).magnitude;
			_closestTime = refP.closestTgtApprUT;

			return true;
		}

		private static bool GetClosestVessel(Orbit refP, Orbit tgtP)
		{
			if (refP == null || tgtP == null)
				return false;

			if (refP.referenceBody != tgtP.referenceBody)
				return false;

			if (!Orbit.PeApIntersects(refP, tgtP, 20000))
				return false;

			double d1 = 0;
			double d2 = 0;
			double dT1 = 0;
			double d4 = 0;
			double dT2 = 0;
			double d6 = 0;
			int i1 = 0;

			int intersects = Orbit.FindClosestPoints(refP, tgtP, ref d1, ref d2, ref dT1, ref d4, ref dT2, ref d6, 0.001, 10, ref i1);

			double UT1 = refP.StartUT + refP.GetDTforTrueAnomaly(dT1, 0);
			double UT2 = refP.StartUT + refP.GetDTforTrueAnomaly(dT2, 0);

			if (intersects > 1)
			{
				double dist1 = double.MaxValue;

				if (PatchedConics.TAIsWithinPatchBounds(UT1, refP))
				{
					Vector3d refClosest1 = refP.getRelativePositionAtUT(UT1);
					Vector3d tgtClosest1 = tgtP.getRelativePositionAtUT(UT1);

					dist1 = (refClosest1 - tgtClosest1).magnitude;
				}

				double dist2 = double.MaxValue;

				if (PatchedConics.TAIsWithinPatchBounds(UT2, refP))
				{
					Vector3d refClosest2 = refP.getRelativePositionAtUT(UT2);
					Vector3d tgtClosest2 = tgtP.getRelativePositionAtUT(UT2);

					dist2 = (refClosest2 - tgtClosest2).magnitude;
				}

				if (dist1 > BasicExtensions.AlmostMaxValue && dist2 > BasicExtensions.AlmostMaxValue)
					return false;

				bool first = dist1 < dist2;

				if (first)
				{
					_closestDist = dist1;
					_closestTime = UT1;

					Vector3d refVel = refP.getOrbitalVelocityAtUT(UT1);
					Vector3d tgtVel = tgtP.getOrbitalVelocityAtUT(UT1);

					_closestRelVel = (refVel - tgtVel).magnitude;
				}
				else
				{
					_closestDist = dist2;
					_closestTime = UT2;

					Vector3d refVel = refP.getOrbitalVelocityAtUT(UT2);
					Vector3d tgtVel = tgtP.getOrbitalVelocityAtUT(UT2);

					_closestRelVel = (refVel - tgtVel).magnitude;
				}

				return true;
			}
			else
			{
				if (!PatchedConics.TAIsWithinPatchBounds(UT1, refP))
					UT1 = double.MaxValue;

				if (!PatchedConics.TAIsWithinPatchBounds(UT2, refP))
					UT2 = double.MaxValue;

				if (UT1 > BasicExtensions.AlmostMaxValue && UT2 > BasicExtensions.AlmostMaxValue)
					return false;

				double useUT = UT1 < UT2 ? UT1 : UT2;

				Vector3d refClosest = refP.getRelativePositionAtUT(useUT);
				Vector3d tgtClosest = tgtP.getRelativePositionAtUT(useUT);

				Vector3d refVel = refP.getOrbitalVelocityAtUT(useUT);
				Vector3d tgtVel = tgtP.getOrbitalVelocityAtUT(useUT);

				_closestDist = (refClosest - tgtClosest).magnitude;
				_closestRelVel = (refVel - tgtVel).magnitude;
				_closestTime = useUT;

				return true;
			}
		}

		private static Orbit GetReferencePatch(OrbitTargeter o, PatchedConicSolver s, CelestialBody tgt, CelestialBody tgtParent, bool vessel)
		{
			if (vessel)
			{
				for (int i = s.patchesAhead; i >= 0; i--)
				{
					Orbit patch = s.patches[i];

					if (patch == null)
						continue;

					if (!patch.activePatch)
						continue;

					if (patch.referenceBody != tgt)
						continue;

					return patch;
				}
			}
			else
			{
				for (int i = s.patchesAhead; i >= 0; i--)
				{
					Orbit patch = s.patches[i];

					if (patch == null)
						continue;

					if (!patch.activePatch)
						continue;

					if (patch.referenceBody == tgt && i > 0)
						return s.patches[i - 1];
					else if (patch.referenceBody == tgtParent)
						return patch;
				}
			}

			return null;
		}

	}
}
