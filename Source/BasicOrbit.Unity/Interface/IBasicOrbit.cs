using System;
using System.Collections.Generic;
using BasicOrbit.Unity.Unity;
using UnityEngine;

namespace BasicOrbit.Unity.Interface
{
	public interface IBasicOrbit
	{
		string Version { get; }

		bool ShowOrbit { get; set; }

		bool ShowTarget { get; set; }

		float Alpha { get; set; }

		float Scale { get; set; }

		float MasterScale { get; }

		IBasicPanel GetOrbitPanel { get; }

		BasicOrbit_Panel GetOrbit { get; }

		IBasicPanel GetTargetPanel { get; }

		BasicOrbit_Panel GetTarget { get; }

		void ProcessStyles(GameObject obj);
	}
}
