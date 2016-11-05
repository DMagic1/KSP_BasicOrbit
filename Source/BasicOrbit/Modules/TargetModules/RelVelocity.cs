#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit RelVelocity - Readout module for velocity relative to target
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
	public class RelVelocity : BasicModule
	{
		public RelVelocity(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showRelVelocity = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showRelVelocityAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			ITargetable tgt = FlightGlobals.ActiveVessel.targetObject;

			if (tgt.GetVessel() != null)
			{
				if (tgt.GetVessel().LandedOrSplashed)
				{
					Vector3d relative = FlightGlobals.ActiveVessel.srf_velocity - tgt.GetSrfVelocity();

					return result(relative.magnitude);
				}
			}

			return result(FlightGlobals.ship_tgtSpeed);
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Speed());
		}
	}
}
