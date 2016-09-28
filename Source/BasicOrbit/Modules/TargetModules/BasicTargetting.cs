using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public static class BasicTargetting
	{
		private static double _distance;
		private static double _closest;
		private static double _closestTime;
		private static double _relVelocity;
		private static double _relInclination;
		private static double _ascendingNode;
		private static double _ascendingTime;
		private static double _descendingNode;
		private static double _descendingTime;
		private static bool _update;

		private static bool _updated;

		private static Orbit _shipOrbit;
		private static Orbit _targetOrbit;

		public static bool Updated
		{
			get { return _updated; }
		}

		public static Orbit ShipOrbit
		{
			get { return _shipOrbit; }
		}

		public static Orbit TargetOrbit
		{
			get { return _targetOrbit; }
		}

		public static double Distance
		{
			get { return _distance; }
		}

		public static double ClosestDistance
		{
			get { return _closest; }
		}

		public static double ClosestTime
		{
			get { return _closestTime; }
		}

		public static double RelVelocity
		{
			get { return _relVelocity; }
		}

		public static double RelInclination
		{
			get { return _relInclination; }
		}

		public static double AscendingNode
		{
			get { return _ascendingNode; }
		}

		public static double AscendingTime
		{
			get { return _ascendingTime; }
		}

		public static double DescendingNode
		{
			get { return _descendingNode; }
		}

		public static double DescendingTime
		{
			get { return _descendingTime; }
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
				return false;

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

			if (active.referenceBody == Planetarium.fetch.Sun || active.referenceBody == FlightGlobals.ActiveVessel.targetObject.GetOrbit().referenceBody)
				_shipOrbit = active;
			else
				_shipOrbit = FlightGlobals.ActiveVessel.orbit.referenceBody.orbit;

			_distance = 0;
			_closest = 0;
			_closestTime = 0;
		}
	}
}
