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

		private static OrbitTargeter.ISectMarker _closestIntersect;
		private static OrbitTargeter.ClApprMarker _approach;

		private static bool _updated;

		private static Orbit _shipOrbit;
		private static Orbit _trueShipOrbit;
		private static Orbit _targetOrbit;

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

		public static OrbitTargeter.ISectMarker ClosestIntersect
		{
			get { return _closestIntersect; }
		}
		
		public static OrbitTargeter.ClApprMarker ApproachMarker
		{
			get { return _approach; }
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
				FlightGlobals.ActiveVessel.orbit.referenceBody == null ||
				FlightGlobals.ActiveVessel.orbit.referenceBody.orbit == null)
			{
				_closestIntersect = null;
				_approach = null;
				_updated = false;
				return false;
			}

			if (FlightGlobals.ActiveVessel.targetObject.GetVessel() == null)
			{
				if (FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null)
				{
					_isCelestial = false;
					_isVessel = false;
				}
				else
				{
					_isCelestial = true;
					_isVessel = false;
				}
			}
			else
			{
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

			//BasicOrbit.BasicLogging("Closest Approach {0:N1}m at {1}", _targetOrbit.ClAppr, KSPUtil.PrintTime(Planetarium.GetUniversalTime() - _targetOrbit.closestTgtApprUT, 3, false));

			//if ((FlightGlobals.ActiveVessel.situation |= Vessel.Situations.LANDED | Vessel.Situations.SPLASHED | Vessel.Situations.PRELAUNCH) == 0)
			//{
			//	_closestDist = 0;
			//	_closestTime = 0;
			//	_closestRelVel = 0;
			//}
			//else
			//{
			//	if (!_isVessel && !_isCelestial)
			//	{
			//		_closestDist = 0;
			//		_closestTime = 0;
			//		_closestRelVel = 0;
			//	}
			//	else
			//	{
			//		double UT = Planetarium.GetUniversalTime();

			//		_closestTime = approachTime(_trueShipOrbit, _targetOrbit, UT);

			//		_closestDist = separation(_trueShipOrbit, _targetOrbit, _closestTime);
			//	}
			//}

			if ((FlightGlobals.ActiveVessel.situation |= Vessel.Situations.LANDED | Vessel.Situations.SPLASHED | Vessel.Situations.PRELAUNCH) == 0)
			{
				_closestIntersect = null;
				_approach = null;
			}
			else
			{
				if (!_isVessel && !_isCelestial)
				{
					_closestIntersect = null;
					_approach = null;
				}
				else
				{
					OrbitTargeter oTarger = FlightGlobals.ActiveVessel.orbitTargeter;

					if (oTarger == null)
					{
						_closestIntersect = null;
						_approach = null;
					}
					else if (!MapView.MapIsEnabled)
					{
						_closestIntersect = null;
						_approach = null;
					}
					else
					{
						if (_isVessel)
						{
							OrbitTargeter.ISectMarker _intersectOne = BasicOrbitLoader.GetIntersect(oTarger, true);
							OrbitTargeter.ISectMarker _intersectTwo = BasicOrbitLoader.GetIntersect(oTarger, false);

							if (_intersectOne != null && _intersectTwo != null)
								_closestIntersect = _intersectOne.separation > _intersectTwo.separation ? _intersectTwo : _intersectOne;
							else if (_intersectOne != null)
								_closestIntersect = _intersectOne;
							else if (_intersectTwo != null)
								_closestIntersect = _intersectTwo;
							else
								_closestIntersect = null;

							_approach = null;
						}
						else if (_isCelestial)
						{
							_closestIntersect = null;
							_approach = BasicOrbitLoader.GetApproach(oTarger);
						}
					}
				}
			}
		}

		private static double approachTime(Orbit a, Orbit b, double UT)
		{
			double closestTime = UT;
			double closest = double.MaxValue;
			double min = UT;
			double interval = a.period;

			if (a.eccentricity > 1)
				interval = 100 / Math.Sqrt(a.referenceBody.gravParameter / Math.Abs(Math.Pow(a.semiMajorAxis, 3)));

			double max = UT + interval;
			int divisions = 20;

			for (int i = 0; i < 8; i++)
			{
				double dt = (max - min) / divisions;

				for (int j = 0; j < divisions; j++)
				{
					double t = min + j * dt;
					double dist = separation(a, b, t);
					if (dist < closest)
					{
						closest = dist;
						closestTime = t;
					}
				}
				min = Clamp(closest - dt, UT, UT + interval);
				max = Clamp(closest + dt, UT, UT + interval);
			}


			return closestTime;
		}

		private static double Clamp(double x, double min, double max)
		{
			if (x < min)
				return min;
			if (x > max)
				return max;
			return x;
		}

		private static double separation(Orbit a, Orbit b, double UT)
		{
			return (SwappedAbsolutePosAtUT(a, UT) - SwappedAbsolutePosAtUT(b, UT)).magnitude;
		}

		private static Vector3d SwappedAbsolutePosAtUT(Orbit a, double UT)
		{
			return a.referenceBody.position + SwappedRelativePosAtUT(a, UT);
		}

		private static Vector3d SwappedRelativePosAtUT(Orbit a, double UT)
		{
			Vector3d v = a.getRelativePositionAtUT(UT);

			return new Vector3d(v.x, v.z, v.y);
		}

	}
}
