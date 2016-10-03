using System;
using System.Collections.Generic;
using System.IO;
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
