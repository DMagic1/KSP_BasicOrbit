using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class ClosestApproach : BasicModule
	{
		private double _cachedDistance;
		private double _cachedTime;
		private bool _cachedVessel;
		private bool _cachedBody;

		public ClosestApproach(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showClosestApproach = IsVisible;
			BasicSettings.Instance.showClosestApproachAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			//return result(BasicTargetting.ClosestDistance, BasicTargetting.ClosestTime - Planetarium.GetUniversalTime());
			
			if (BasicTargetting.IsVessel)
			{
				if (MapView.MapIsEnabled)
				{
					if (BasicTargetting.ClosestIntersect == null)
					{
						_cachedVessel = false;
						_cachedBody = false;
						_cachedDistance = 0;
						_cachedTime = 0;
						return "---";
					}

					double distance = BasicTargetting.ClosestIntersect.separation * 1000;
					double time = BasicTargetting.ClosestIntersect.UT;

					_cachedVessel = true;
					_cachedBody = false;
					_cachedDistance = distance;
					_cachedTime = time;

					return result(distance, time - Planetarium.GetUniversalTime());
				}
				else if (_cachedVessel)
					return "~" + result(_cachedDistance, _cachedTime - Planetarium.GetUniversalTime());

				_cachedVessel = false;
				_cachedBody = false;
				_cachedDistance = 0;
				_cachedTime = 0;
				return "---";
			}
			else if (BasicTargetting.IsCelestial)
			{
				if (MapView.MapIsEnabled)
				{
					if (BasicTargetting.ApproachMarker == null)
					{
						_cachedVessel = false;
						_cachedBody = false;
						_cachedDistance = 0;
						_cachedTime = 0;
						return "---";
					}

					double distance = BasicTargetting.ApproachMarker.separation * 1000;
					double time = -1 * BasicTargetting.ApproachMarker.dT;

					_cachedVessel = false;
					_cachedBody = true;
					_cachedDistance = distance;
					_cachedTime = time + Planetarium.GetUniversalTime();

					return result(distance, time);
				}
				else if (_cachedBody)
					return "~" + result(_cachedDistance, _cachedTime - Planetarium.GetUniversalTime());

				_cachedVessel = false;
				_cachedBody = false;
				_cachedDistance = 0;
				_cachedTime = 0;
				return "---";
			}

			_cachedVessel = false;
			_cachedBody = false;
			_cachedDistance = 0;
			_cachedTime = 0;
			return "---";
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.CloseDistance(), KSPUtil.PrintTime(t, 3, false));
		}
	}
}
