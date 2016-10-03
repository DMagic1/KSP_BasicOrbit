using System;
using System.Collections.Generic;
using BasicOrbit.Unity.Unity;
using UnityEngine;

namespace BasicOrbit.Unity.Interface
{
	public interface IBasicModule
	{
		string ModuleTitle { get; }

		string ModuleText { get; }

		bool IsVisible { get; set; }

		bool IsActive { get; set; }

		bool AlwaysShow { get; set; }

		void Update();
	}
}
