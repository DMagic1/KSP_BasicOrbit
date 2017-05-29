#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit_SettingsModule - Script for controlling the readout module settings elements
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

using BasicOrbit.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity.Unity
{
	/// <summary>
	/// This class is used for controlling the visibility of a readout module
	/// </summary>
	public class BasicOrbit_SettingsModule : MonoBehaviour
	{
		[SerializeField]
		private TextHandler m_Title = null;
		[SerializeField]
		private StateToggle m_Toggle = null;
		[SerializeField]
		private StateToggle m_ShowAlways = null;

		private IBasicModule moduleInterface;
		private bool loaded;

		/// <summary>
		/// This method is used to initialize the UI element with the readout module
		/// </summary>
		/// <param name="module">The readout module interface</param>
		public void setModule(IBasicModule module)
		{
			if (module == null || m_Title == null || m_Toggle == null || m_ShowAlways == null)
				return;

			moduleInterface = module;

			m_Title.OnTextUpdate.Invoke(module.ModuleTitle);

			m_Toggle.isOn = module.IsVisible;

			m_ShowAlways.isOn = module.AlwaysShow;

			loaded = true;
		}

		/// <summary>
		/// This toggle controls the standard visibility setting for the readout module
		/// </summary>
		/// <param name="isOn">Display is on?</param>
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

		/// <summary>
		/// This toggle forces the readout module to always be displayed, regardless of situation
		/// </summary>
		/// <param name="isOn">Display is forced on?</param>
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
