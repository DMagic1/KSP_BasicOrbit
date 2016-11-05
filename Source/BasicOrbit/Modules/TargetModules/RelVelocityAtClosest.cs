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
		public RelVelocityAtClosest(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showClosestApproachVelocity = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showClosestApproachVelocityAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicTargetting.Updated)
				return "---";

			if (BasicTargetting.VesselIntersect)
				return result(BasicTargetting.ClosestRelVelocity);

			return "---";
		}

		private string result(double d)
		{
			return string.Format("{0}", d.Speed());
		}
	}
}
