using System;
using System.Collections.Generic;
using UnityEngine;

namespace BasicOrbit.Unity.Interface
{
	public interface IBasicPanel
	{
		bool IsVisible { get; set; }

		bool AnyActive { get; }

		float Alpha { get; }

		float Scale { get; }
		
		Vector2 Position { get; set; }

		IList<IBasicModule> GetModules { get; }

		void ClampToScreen(RectTransform rect);
	}
}
