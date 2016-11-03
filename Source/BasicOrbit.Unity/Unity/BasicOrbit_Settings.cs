#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit_Settings - Script for controlling the settings panel
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

namespace BasicOrbit.Unity.Unity
{
	/// <summary>
	/// This class controls the creation of the settings panels and populates it with the available modules
	/// </summary>
	public class BasicOrbit_Settings : CanvasFader
	{
		[SerializeField]
		private GameObject m_SettingModulePrefab = null;
		[SerializeField]
		private Transform m_SettingModuleTransform = null;
		[SerializeField]
		private TextHandler m_Title = null;

		private IList<IBasicModule> sectionModules;

		/// <summary>
		/// This method is used to initialize the panel
		/// </summary>
		/// <param name="modules">The list of readout module types associated with this panel</param>
		/// <param name="t">The title of the panel</param>
		public void createSettings(IList<IBasicModule> modules, string t)
		{
			if (modules == null)
				return;

			if (m_Title != null)
				m_Title.OnTextUpdate.Invoke(t);

			sectionModules = modules;

			CreateModules(sectionModules);
		}

		private void CreateModules(IList<IBasicModule> modules)
		{
			if (m_SettingModulePrefab == null || m_SettingModuleTransform == null)
				return;

			if (modules == null)
				return;

			for (int i = modules.Count - 1; i >= 0; i--)
			{
				IBasicModule module = modules[i];

				if (module == null)
					continue;

				CreateModule(module);
			}
		}

		private void CreateModule(IBasicModule module)
		{
			GameObject mod = Instantiate(m_SettingModulePrefab);

			if (mod == null)
				return;

			mod.transform.SetParent(m_SettingModuleTransform, false);

			BasicOrbit_SettingsModule bMod = mod.GetComponent<BasicOrbit_SettingsModule>();

			if (bMod == null)
				return;

			bMod.setModule(module);
		}
	}
}
