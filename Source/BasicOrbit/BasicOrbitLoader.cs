using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BasicOrbit
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class BasicOrbitLoader : MonoBehaviour
	{
		private static bool loaded;

		private static FieldInfo intersectMarker1 = null;
		private static FieldInfo intersectMarker2 = null;
		private static FieldInfo approachMarker = null;

		public static OrbitTargeter.ISectMarker GetIntersect(OrbitTargeter targeter, bool one)
		{
			if (targeter == null)
				return null;

			if (intersectMarker1 == null || intersectMarker2 == null)
				return null;

			try
			{
				OrbitTargeter.ISectMarker marker = (one ? intersectMarker1.GetValue(targeter) : intersectMarker2.GetValue(targeter)) as OrbitTargeter.ISectMarker;
				return marker;
			}
			catch (Exception e)
			{
				Debug.LogWarning("[Basic Orbit] Error while assigning Orbit Targeter Intersect Marker\n" + e);
				return null;
			}
		}

		public static OrbitTargeter.ClApprMarker GetApproach(OrbitTargeter targeter)
		{
			if (targeter == null)
				return null;

			if (approachMarker == null)
				return null;

			try
			{
				OrbitTargeter.ClApprMarker marker = approachMarker.GetValue(targeter) as OrbitTargeter.ClApprMarker;
				return marker;
			}
			catch (Exception e)
			{
				Debug.LogWarning("[Basic Orbit] Error while assigning Orbit Targeter Approach Marker\n" + e);
				return null;
			}
		}

		private static AssetBundle prefabs;

		public static AssetBundle Prefabs
		{
			get { return prefabs; }
		}

		private void Awake()
		{
			if (loaded)
				Destroy(gameObject);

			string path = KSPUtil.ApplicationRootPath + "GameData/BasicOrbit/Resources";

			prefabs = AssetBundle.LoadFromFile(path + "/basic_orbit_prefabs.ksp");

			loadReflection();

			loaded = true;

			Destroy(gameObject);
		}

		private void loadReflection()
		{
			Type OT = typeof(OrbitTargeter);

			var fields = OT.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);

			List<FieldInfo> intersects = new List<FieldInfo>();
			List<FieldInfo> approaches = new List<FieldInfo>();

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo f = fields[i];

				var returnType = f.FieldType;

				if (returnType == typeof(OrbitTargeter.ISectMarker))
					intersects.Add(f);
				else if (returnType == typeof(OrbitTargeter.ClApprMarker))
					approaches.Add(f);
			}

			if (intersects.Count >= 3)
			{
				intersectMarker1 = intersects[0];
				intersectMarker2 = intersects[2];
			}

			if (approaches.Count > 0)
				approachMarker = approaches[0];
		}
	}
}
