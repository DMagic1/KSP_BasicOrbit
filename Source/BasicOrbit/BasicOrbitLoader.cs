using System.IO;
using System.Reflection;
using UnityEngine;

namespace BasicOrbit
{
	[KSPAddon(KSPAddon.Startup.Instantly, true)]
	public class BasicOrbitLoader : MonoBehaviour
	{
		private static bool loaded;

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

			loaded = true;

			Destroy(gameObject);
		}
	}
}
