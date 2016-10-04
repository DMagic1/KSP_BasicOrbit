#region License
/*
 * Basic Orbit
 * 
 * BasicOrbitReflection - MonoBehaviour for assigning method through reflection
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
using System.Reflection;
using UnityEngine;

namespace BasicOrbit
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class BasicOrbitReflection : MonoBehaviour
	{
		private static bool reflectionLoaded;

		private static FieldInfo orbitMarkers = null;

		public static List<OrbitTargeter.Marker> GetOrbitMarkers(OrbitTargeter targeter)
		{
			if (targeter == null)
				return null;

			if (orbitMarkers == null)
				return null;

			try
			{
				return orbitMarkers.GetValue(targeter) as List<OrbitTargeter.Marker>;
			}
			catch (Exception e)
			{
				Debug.LogWarning("[Basic Orbit] Error while assigning Orbit Targeter Markers\n" + e);
				return null;
			}
		}

		private void Awake()
		{
			if (reflectionLoaded)
				Destroy(gameObject);

			orbitMarkers = typeof(OrbitTargeter).GetField("markers", BindingFlags.NonPublic | BindingFlags.Instance);

			reflectionLoaded = true;

			Destroy(gameObject);
		}

	}
}
