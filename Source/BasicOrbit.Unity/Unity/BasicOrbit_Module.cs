#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit_Module - Script for controlling the readout module
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

namespace BasicOrbit.Unity.Unity
{
	/// <summary>
	/// This class controls the readout module text
	/// </summary>
	public class BasicOrbit_Module : MonoBehaviour
	{
		[SerializeField]
		private TextHandler m_Title = null;
		[SerializeField]
		private TextHandler m_TextModule = null;

		private IBasicModule moduleInterface;

		/// <summary>
		/// This method is used to initialize the readout module; sets the readout title field
		/// </summary>
		/// <param name="module">The readout module interface</param>
		public void setModule(IBasicModule module)
		{
			if (module == null || m_Title == null || m_TextModule == null)
				return;

			moduleInterface = module;

			m_Title.OnTextUpdate.Invoke(module.ModuleTitle + ":");
		}

		/// <summary>
		/// Public property for accessing the visibility status of this module; visibility is controlled through the settings panel
		/// </summary>
		public bool IsVisible
		{
			get
			{
				if (moduleInterface == null)
					return false;

				return moduleInterface.IsVisible;
			}
		}

		/// <summary>
		/// Public property for accessing the active status of the module; this status is updated based on the current vessel status and situation
		/// </summary>
		public bool IsActive
		{
			get
			{
				if (moduleInterface == null)
					return false;

				return moduleInterface.IsActive;
			}
		}

		/// <summary>
		/// Public property for accessing the forced visibility status of this module; this status is controlled through the settings panel
		/// </summary>
		public bool AlwaysShow
		{
			get
			{
				if (moduleInterface == null)
					return false;

				return moduleInterface.AlwaysShow;
			}
		}

		/// <summary>
		/// Method used to update to upstream readout module controller and to update the readout text field
		/// </summary>
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

			m_TextModule.OnTextUpdate.Invoke(moduleInterface.ModuleText);
		}
	}
}
