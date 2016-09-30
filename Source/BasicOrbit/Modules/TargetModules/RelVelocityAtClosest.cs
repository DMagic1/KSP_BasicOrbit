using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Modules.TargetModules
{
	public class RelVelocityAtClosest : BasicModule
	{
		private double _cachedVelocity;
		private bool _cached;

		public RelVelocityAtClosest(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showClosestApproachVelocity = IsVisible;
			BasicSettings.Instance.showClosestApproachVelocityAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			if (BasicTargetting.IsVessel)
			{
				if (MapView.MapIsEnabled)
				{
					if (BasicTargetting.ClosestIntersect == null)
					{
						_cached = false;
						_cachedVelocity = 0;
						return "---";
					}

					double vel = BasicTargetting.ClosestIntersect.relSpeed;

					_cached = true;
					_cachedVelocity = vel;

					return result(vel);
				}
				else if (_cached)
					return "~" + result(_cachedVelocity);

				_cached = false;
				_cachedVelocity = 0;
				return "---";
			}

			_cached = false;
			_cachedVelocity = 0;
			return "---";
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Speed());
		}
	}
}
