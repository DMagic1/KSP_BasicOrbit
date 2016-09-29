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
				return string.Format("{0:N0}m", d);
			else if (d < 1000000000)
				return string.Format("{0:N1}km", d / 1000);
			else
				return string.Format("{0:N2}Mm", d / 1000000);
		}

		public static string CloseDistance(this double d)
		{
			if (Math.Abs(d) < 1000)
			{
				if (Math.Abs(d) > 10)
					return string.Format("{0:N1}m", d);
				else if (Math.Abs(d) > 0.1)
					return string.Format("{0:N1}cm", d * 100);
				else
					return string.Format("{0:N0}mm", d * 1000);
			}
			else if (d < 1000000000)
				return string.Format("{0:N1}km", d / 1000);
			else
				return string.Format("{0:N2}Mm", d / 1000000);
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
