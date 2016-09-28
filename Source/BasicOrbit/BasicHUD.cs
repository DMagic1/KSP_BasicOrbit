using System;
using System.Collections.Generic;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using KSP.UI;

namespace BasicOrbit
{
	public class BasicHUD : IBasicPanel
	{
		private bool _isVisible;
		private List<IBasicModule> modules = new List<IBasicModule>();
		private Vector2 position = new Vector2();

		public BasicHUD(List<IBasicModule> m)
		{
			modules = m;
		}

		public bool IsVisible
		{
			get { return _isVisible; }
			set { _isVisible = value; }
		}

		public bool AnyActive
		{
			get
			{
				bool b = false;

				for (int i = modules.Count - 1; i >= 0; i--)
				{
					IBasicModule module = modules[i];

					if (module == null)
						continue;

					if (!module.IsActive)
						continue;

					b = true;
					break;
				}

				return b;
			}
		}

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public float Alpha
		{
			get { return BasicSettings.Instance.panelAlpha; }
		}

		public float Scale
		{
			get { return BasicSettings.Instance.UIScale; }
		}
		
		public IList<IBasicModule> GetModules
		{
			get { return new List<IBasicModule>(modules.ToArray()); }
		}

		public void ProcessStyles(GameObject obj)
		{
			BasicOrbitUtilities.processComponents(obj);
		}

		public void ClampToScreen(RectTransform rect)
		{
			UIMasterController.ClampToScreen(rect, Vector2.zero);
		}
	}
}
