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

			if (BasicTargetting.VesselIntersect || BasicTargetting.BodyIntersect)
				return result(BasicTargetting.ClosestDistance, BasicTargetting.ClosestTime - Planetarium.GetUniversalTime());

			return "---";
		}

		private string result(double d, double t)
		{
			return string.Format("{0} in {1}", d.Distance(), t.Time(2));
		}
	}
}
