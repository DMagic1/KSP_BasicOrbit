using System;
using System.Collections.Generic;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity.Unity
{
	public class BasicOrbit_Settings : CanvasGrower
	{
		[SerializeField]
		private GameObject m_SettingModulePrefab = null;
		[SerializeField]
		private Transform m_SettingModuleTransform = null;
		[SerializeField]
		private Text m_Title = null;

		private IList<IBasicModule> sectionModules;

		public void createSettings(IList<IBasicModule> modules, string t)
		{
			if (modules == null)
				return;

			if (m_Title != null)
				m_Title.text = t;

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

			module.ProcessStyles(mod);

			mod.transform.SetParent(m_SettingModuleTransform, false);

			BasicOrbit_SettingsModule bMod = mod.GetComponent<BasicOrbit_SettingsModule>();

			if (bMod == null)
				return;

			bMod.setModule(module);
		}
	}
}
