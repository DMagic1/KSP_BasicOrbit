#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit TimeToEqNode - Readout module for time to equatorial crossing node
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

using UnityEngine;

namespace BasicOrbit.Modules.OrbitModules
{
	public class TimeToEqNode : BasicModule
	{
		private bool asc;

		public TimeToEqNode(string t)
			: base(t)
		{

		}

		protected override void UpdateVisible()
		{
			BasicSettings.Instance.showTimeToEqNode = IsVisible;
		}

		protected override void UpdateAlways()
		{
			BasicSettings.Instance.showTimeToEqNodeAlways = AlwaysShow;
		}
		
		protected override string fieldUpdate()
		{
			if (FlightGlobals.ActiveVessel == null)
				return "---";

			if (FlightGlobals.ActiveVessel.orbit == null)
				return "---";

			return result(getNextNode(FlightGlobals.ActiveVessel.orbit));
		}


		private double getNextNode(Orbit o)
		{
			double period = 0;

			if (!double.IsNaN(o.period) && !double.IsInfinity(o.period))
				period = o.period;

			double eqAsc = o.GetDTforTrueAnomaly((360 - o.argumentOfPeriapsis) * Mathf.Deg2Rad, period);

			double eqDesc = o.GetDTforTrueAnomaly((180 - o.argumentOfPeriapsis) * Mathf.Deg2Rad, period);

			if (eqAsc < 0)
				eqAsc += period;
			else if (eqAsc > period)
				eqAsc -= period;

			if (eqDesc < 0)
				eqDesc += period;
			else if (eqDesc > period)
				eqDesc -= period;

			asc = eqAsc < eqDesc;

			return asc ? eqAsc : eqDesc;
		}

		private string result(double t)
		{
			return string.Format("{0} node in {1}", asc ? "Asc" : "Desc", t.Time(2));
		}
	}
}
