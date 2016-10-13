#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit RelVelocityAtClosest - Readout module for velocity relative to target at closest approach
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

		public bool Cached
		{
			get { return _cached; }
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
					if (!BasicTargetting.VesselIntersect)
					{
						_cached = false;
						_cachedVelocity = 0;
						return "---";
					}

					double vel = BasicTargetting.ClosestRelVelocity;

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
