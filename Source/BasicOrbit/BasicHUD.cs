#region License
/*
 * Basic Orbit
 * 
 * BasicHUD - Storage class for holding information on the readout module panels
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

					if (module.IsVisible || module.AlwaysShow)
					{
						b = true;
						break;
					}
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

		public void ClampToScreen(RectTransform rect)
		{
			UIMasterController.ClampToScreen(rect, Vector2.zero);
		}
	}
}
