#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit BurnTime - Readout module for maneuver start burn time
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

namespace BasicOrbit.Modules.ManeuverModules
{
	public class BurnTime : BasicModule
	{
		public BurnTime(string t)
			: base(t)
		{

		}

		protected override void UpdateSettings()
		{
			BasicSettings.Instance.showManeuverBurn = IsVisible;
			BasicSettings.Instance.showManeuverBurnAlways = AlwaysShow;
		}

		protected override string fieldUpdate()
		{
			if (!BasicManeuvering.Updated)
				return "---";

			if (BasicManeuvering.BBTimeLoaded)
				return bbResult(BasicManeuvering.BurnLength, BasicManeuvering.BurnTime);

			return result(BasicManeuvering.BurnTime - Planetarium.GetUniversalTime());
		}

		private string result(double t)
		{
			return t.Time(3);
		}

		private string bbResult(double l, double t)
		{
			return string.Format("Start {0} burn in {1}", l.Time(2), t.Time(2));
		}
	}
}
