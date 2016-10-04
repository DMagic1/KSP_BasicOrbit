#region License
/*
 * Basic Orbit
 * 
 * BasicExtensions - A few extensions for printing values in the correct units
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

using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit
{
	public static class BasicExtensions
	{
		public static string Distance(this double d)
		{
			if (d < 1000000)
				return string.Format("{0:N2}m", d);
			else if (d < 1000000000)
				return string.Format("{0:N1}km", d / 1000);
			else
				return string.Format("{0:N1}Mm", d / 1000000);
		}

		public static string CloseDistance(this double d)
		{
			if (Math.Abs(d) < 10000)
			{
				if (Math.Abs(d) > 10)
					return string.Format("{0:N3}m", d);
				else if (Math.Abs(d) > 0.1)
					return string.Format("{0:N2}cm", d * 100);
				else
					return string.Format("{0:N0}mm", d * 1000);
			}
			else if (d < 1000000000)
				return string.Format("{0:N1}km", d / 1000);
			else
				return string.Format("{0:N1}Mm", d / 1000000);
		}

		public static string Speed(this double d)
		{
			if (Math.Abs(d) < 1)
				return string.Format("{0:N2}cm/s", d * 100);
			else if (Math.Abs(d) < 1000)
				return string.Format("{0:N2}m/s", d);
			else
				return string.Format("{0:N3}km/s", d / 1000);
		}

	}
}
