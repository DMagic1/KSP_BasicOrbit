using System;
using System.Collections.Generic;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity.Unity
{
	public class BasicOrbit_SettingsModule : MonoBehaviour
	{
		[SerializeField]
		private Text m_Title = null;
		[SerializeField]
		private Toggle m_Toggle = null;
		[SerializeField]
		private Toggle m_ShowAlways = null;

		private IBasicModule moduleInterface;
		private bool loaded;

		public void setModule(IBasicModule module)
		{
			if (module == null || m_Title == null || m_Toggle == null || m_ShowAlways == null)
				return;

			moduleInterface = module;

			m_Title.text = module.ModuleTitle;

			m_Toggle.isOn = module.IsVisible;

			m_ShowAlways.isOn = module.AlwaysShow;

			loaded = true;
		}

		public void VisibleToggle(bool isOn)
		{
			if (!loaded)
				return;

			if (moduleInterface == null)
				return;

			moduleInterface.IsVisible = isOn;

			if (!isOn && m_ShowAlways != null)
				m_ShowAlways.isOn = isOn;
		}

		public void AlwaysToggle(bool isOn)
		{
			if (!loaded)
				return;

			if (moduleInterface == null)
				return;

			moduleInterface.AlwaysShow = isOn;

			if (isOn && m_Toggle != null)
				m_Toggle.isOn = isOn;
		}
	}
}
