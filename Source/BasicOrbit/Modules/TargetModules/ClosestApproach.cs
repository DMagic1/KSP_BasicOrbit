#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit ClosestApproach - Readout module for closest approach to target
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

			if (BasicTargetting.IsVessel)
			{
				if (MapView.MapIsEnabled)
				{
					if (!BasicTargetting.VesselIntersect)
					{
						_cachedVessel = false;
						_cachedBody = false;
						_cachedDistance = 0;
						_cachedTime = 0;
						return "---";
					}

					double distance = BasicTargetting.ClosestDistance;
					double time = BasicTargetting.ClosestTime;

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
					if (!BasicTargetting.BodyIntersect)
					{
						_cachedVessel = false;
						_cachedBody = false;
						_cachedDistance = 0;
						_cachedTime = 0;
						return "---";
					}

					double distance = BasicTargetting.ClosestDistance;
					double time = BasicTargetting.ClosestTime;

					_cachedVessel = false;
					_cachedBody = true;
					_cachedDistance = distance;
					_cachedTime = time;

					return result(distance, time - Planetarium.GetUniversalTime());
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
			return string.Format("{0} in {1}", d.Distance(), t.Time(2));
		}
	}
}
