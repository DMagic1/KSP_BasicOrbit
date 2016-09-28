using System;
using System.Collections.Generic;
using System.Linq;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity.Unity
{

	public class BasicOrbit_Module : MonoBehaviour
	{
		[SerializeField]
		private Text m_Title = null;
		[SerializeField]
		private Text m_TextModule = null;

		private IBasicModule moduleInterface;

		public void setModule(IBasicModule module)
		{
			if (module == null || m_Title == null || m_TextModule == null)
				return;

			moduleInterface = module;

			m_Title.text = module.ModuleTitle;
		}

		public void Toggle(bool isOn)
		{
			gameObject.SetActive(isOn);
		}

		public bool IsVisible
		{
			get
			{
				if (moduleInterface == null)
					return false;

				return moduleInterface.IsVisible;
			}
		}

		public bool IsActive
		{
			get
			{
				if (moduleInterface == null)
					return false;

				return moduleInterface.IsActive;
			}
		}

		public bool AlwaysShow
		{
			get
			{
				if (moduleInterface == null)
					return false;

				return moduleInterface.AlwaysShow;
			}
		}

		public void ToggleVisibility(bool isOn)
		{
			if (moduleInterface == null)
				return;

			moduleInterface.IsVisible = isOn;

			gameObject.SetActive(isOn);
		}

		public void UpdateModule()
		{
			if (moduleInterface == null)
				return;

			if (!moduleInterface.IsVisible)
				return;

			if (!moduleInterface.IsActive && !moduleInterface.AlwaysShow)
				return;
			
			if (m_TextModule == null)
				return;

			moduleInterface.Update();

			m_TextModule.text = moduleInterface.ModuleText;
		}
	}
}
