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
using System.Text;
using UnityEngine;

namespace BasicOrbit
{
	public static class BasicExtensions
	{
		private static int[] times = new int[5];
		private static string[] units = new string[5] { "s", "m", "h", "d", "y" };

		public static string Distance(this double d)
		{
			if (d < 1000000)
				return string.Format("{0:N2}m", d);
			else if (d < 1000000000)
				return string.Format("{0:N1}km", d / 1000);
			else if (d < 1000000000000)
				return string.Format("{0:N1}Mm", d / 1000000);
			else
				return string.Format("{0:N0}Gm", d / 1000000000);
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
				return string.Format("{0:N0}Mm", d / 1000000);
		}

		public static string Speed(this double d)
		{
			if (Math.Abs(d) < 1)
				return string.Format("{0:N3}cm/s", d * 100);
			else if (Math.Abs(d) < 1000)
				return string.Format("{0:N2}m/s", d);
			else
				return string.Format("{0:N2}km/s", d / 1000);
		}

		public static string Time(this double d, int values)
		{
			d.SetTimes();

			StringBuilder sb = StringBuilderCache.Acquire();

			if (d < 0)
				sb.Append("- ");

			for (int i = times.Length -1; i >= 0; i--)
			{
				int t = times[i];

				if (t == 0)
				{
					if (i < times.Length - 1 && times[i + 1] == 0)
						continue;
					else if (i >= times.Length - 1)
						continue;
				}

				if (values <= 0)
					continue;

				string format = "";

				if (i <= 1)
					format = "{0:D2}{1}";
				else if (i == 2 && times[3] != 0)
					format = "{0:D2}{1}";
				else
					format = "{0}{1}";

				sb.Append(String.Format(format, Math.Abs(t), units[i]));

				if (values > 1)
					sb.Append(", ");

				values--;
			}

			return sb.ToStringAndRelease();
		}

		private static void SetTimes(this double d)
		{
			int year = KSPUtil.dateTimeFormatter.Year;
			int day = KSPUtil.dateTimeFormatter.Day;

			times[4] = (int)(d / year);
			d -= times[4] * year;

			times[3] = (int)(d / day);
			d -= times[3] * day;

			times[2] = (int)(d / 3600);
			d -= times[2] * 3600;

			times[1] = (int)(d / 60);
			d -= times[1] * 60;

			times[0] = (int)d;
		}


	}
}
