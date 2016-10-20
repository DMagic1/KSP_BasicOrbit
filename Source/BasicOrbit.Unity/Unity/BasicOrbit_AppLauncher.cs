#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit_AppLauncher - Script for controlling the main toolbar panel
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
	/// This class controls the main toolbar settings window UI element
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class BasicOrbit_AppLauncher : CanvasFader
	{
		[SerializeField]
		private GameObject m_SettingsPrefab = null;
		[SerializeField]
		private Toggle m_OrbitToggle = null;
		[SerializeField]
		private Toggle m_TargetToggle = null;
		[SerializeField]
		private Toggle m_ManeuverToggle = null;
		[SerializeField]
		private Toggle m_OrbitDragToggle = null;
		[SerializeField]
		private Toggle m_TargetDragToggle = null;
		[SerializeField]
		private Toggle m_ManeuverDragToggle = null;
		[SerializeField]
		private Toggle m_OrbitSettingsToggle = null;
		[SerializeField]
		private Toggle m_TargetSettingsToggle = null;
		[SerializeField]
		private Toggle m_ManeuverSettingsToggle = null;
		[SerializeField]
		private TextHandler m_AlphaText = null;
		[SerializeField]
		private TextHandler m_ScaleText = null;
		[SerializeField]
		private Slider m_AlphaSlider = null;
		[SerializeField]
		private Slider m_ScaleSlider = null;
		[SerializeField]
		private TextHandler m_VersionText = null;

		private BasicOrbit_Settings orbitSettings;
		private BasicOrbit_Settings targetSettings;
		private BasicOrbit_Settings maneuverSettings;

		private IBasicOrbit basicInterface;
		private RectTransform rect;

		private bool loaded;

		private void Start()
		{
			rect = GetComponent<RectTransform>();

			Alpha(0);

			Fade(1, true);
		}

		/// <summary>
		/// Fade out the panel on closing; cancel any dragging states for the readout panels
		/// </summary>
		public void Close()
		{
			Fade(0, true, Kill, false);

			if (basicInterface == null)
				return;

			if (basicInterface.GetOrbit != null)
				basicInterface.GetOrbit.Dragging = false;

			if (basicInterface.GetTarget != null)
				basicInterface.GetTarget.Dragging = false;

			if (basicInterface.GetManeuver != null)
				basicInterface.GetManeuver.Dragging = false;
		}

		private void Kill()
		{
			gameObject.SetActive(false);

			Destroy(gameObject);
		}

		/// <summary>
		/// This method is used to initialize the panel
		/// </summary>
		/// <param name="basic">The main interface module</param>
		public void setOrbit(IBasicOrbit basic)
		{
			if (basic == null || m_OrbitToggle == null || m_TargetToggle == null)
				return;

			basicInterface = basic;

			m_OrbitToggle.isOn = basic.ShowOrbit;

			m_TargetToggle.isOn = basic.ShowTarget;

			m_ManeuverToggle.isOn = basic.ShowManeuver;

			if (m_AlphaText != null && m_AlphaSlider != null)
			{
				m_AlphaText.OnTextUpdate.Invoke(basic.Alpha.ToString("P0"));

				m_AlphaSlider.value = basic.Alpha * 50;
			}

			if (m_ScaleText != null && m_ScaleSlider != null)
			{
				m_ScaleText.OnTextUpdate.Invoke(basic.Scale.ToString("P0"));

				m_ScaleSlider.value = basic.Scale * 10;
			}

			if (m_VersionText != null)
				m_VersionText.OnTextUpdate.Invoke(basic.Version);

			transform.localScale = Vector3.one * basic.Scale;

			loaded = true;
		}

		/// <summary>
		/// Toggle to control the visibility of the orbit readout panel
		/// </summary>
		/// <param name="isOn">Display the panel?</param>
		public void ToggleOrbitPanel(bool isOn)
		{
			if (!loaded)
				return;

			if (basicInterface == null)
				return;

			basicInterface.ShowOrbit = isOn;

			//Disable dragging toggle if it is active when the panel is closed
			if (!isOn && m_OrbitDragToggle != null)
				m_OrbitDragToggle.isOn = false;
		}

		/// <summary>
		/// Toggle to control the visibility of the target readout panel
		/// </summary>
		/// <param name="isOn">Display the panel?</param>
		public void ToggleTargetPanel(bool isOn)
		{
			if (!loaded)
				return;

			if (basicInterface == null)
				return;

			basicInterface.ShowTarget = isOn;

			//Disable dragging toggle if it is active when the panel is closed
			if (!isOn && m_TargetDragToggle != null)
				m_TargetDragToggle.isOn = false;
		}

		/// <summary>
		/// Toggle to control the visibility of the maneuver readout panel
		/// </summary>
		/// <param name="isOn">Display the panel?</param>
		public void ToggleManueverPanel(bool isOn)
		{
			if (!loaded)
				return;

			if (basicInterface == null)
				return;

			basicInterface.ShowManeuver = isOn;

			//Disable dragging toggle if it is active when the panel is closed
			if (!isOn && m_ManeuverDragToggle != null)
				m_ManeuverDragToggle.isOn = false;
		}

		/// <summary>
		/// Toggle to control the drag state of the orbit readout panel
		/// </summary>
		/// <param name="isOn">Drag state active?</param>
		public void ToggleOrbitDrag(bool isOn)
		{
			if (basicInterface == null)
				return;

			if (basicInterface.GetOrbit == null)
				return;

			//If the orbit panel is closed activate it
			if (isOn && basicInterface.GetOrbitPanel != null)
			{
				if (!basicInterface.GetOrbitPanel.IsVisible && m_OrbitToggle != null)
					m_OrbitToggle.isOn = true;
			}

			basicInterface.GetOrbit.Dragging = isOn;
		}

		/// <summary>
		/// Toggle to control the drag state of the target readout panel
		/// </summary>
		/// <param name="isOn">Drag state active?</param>
		public void ToggleTargetDrag(bool isOn)
		{
			if (basicInterface == null)
				return;

			if (basicInterface.GetTarget == null)
				return;

			//If the target panel is closed activate it
			if (isOn && basicInterface.GetTargetPanel != null)
			{
				if (!basicInterface.GetTargetPanel.IsVisible && m_TargetToggle != null)
					m_TargetToggle.isOn = true;
			}

			basicInterface.GetTarget.Dragging = isOn;
		}

		/// <summary>
		/// Toggle to control the drag state of the maneuver readout panel
		/// </summary>
		/// <param name="isOn">Drag state active?</param>
		public void ToggleManeuverDrag(bool isOn)
		{
			if (basicInterface == null)
				return;

			if (basicInterface.GetManeuver == null)
				return;

			//If the target panel is closed activate it
			if (isOn && basicInterface.GetManeuverPanel != null)
			{
				if (!basicInterface.GetManeuverPanel.IsVisible && m_ManeuverToggle != null)
					m_ManeuverToggle.isOn = true;
			}

			basicInterface.GetManeuver.Dragging = isOn;
		}

		/// <summary>
		/// Toggle to display the orbit readout settings panel
		/// </summary>
		/// <param name="isOn">Display the settings panel?</param>
		public void ShowOrbitSettings(bool isOn)
		{
			//Destroy the settings panel if it is already active
			if (orbitSettings != null)
			{
				orbitSettings.gameObject.SetActive(false);

				Destroy(orbitSettings);
				return;
			}

			if (!isOn)
				return;

			if (m_TargetSettingsToggle != null)
				m_TargetSettingsToggle.isOn = false;

			if (m_ManeuverSettingsToggle != null)
				m_ManeuverSettingsToggle.isOn = false;

			if (m_SettingsPrefab == null)
				return;

			GameObject obj = Instantiate(m_SettingsPrefab);

			if (obj == null)
				return;

			obj.transform.SetParent(transform, false);

			orbitSettings = obj.GetComponent<BasicOrbit_Settings>();

			if (orbitSettings == null)
				return;

			orbitSettings.createSettings(basicInterface.GetOrbitPanel.GetModules, "Orbit Panel Settings");

			//Position the panel above or below the main settings window depending on its position on screen
			RectTransform r = orbitSettings.GetComponent<RectTransform>();

			if (rect.position.y - (125 * basicInterface.Scale * basicInterface.MasterScale) < 0)
			{
				float height = 358 * basicInterface.Scale * basicInterface.MasterScale;

				r.position = new Vector3(rect.position.x, rect.position.y + height, rect.position.z);
			}
			else
			{
				float y = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

				r.position = new Vector3(rect.position.x, rect.position.y - y, rect.position.z);
			}
		}

		/// <summary>
		/// Toggle to display the target readout settings panel
		/// </summary>
		/// <param name="isOn">Display the settings panel?</param>
		public void ShowTargetSettings(bool isOn)
		{
			//Destroy the settings panel if it is already active
			if (targetSettings != null)
			{
				targetSettings.gameObject.SetActive(false);

				Destroy(targetSettings);
				return;
			}

			if (!isOn)
				return;

			if (m_OrbitSettingsToggle != null)
				m_OrbitSettingsToggle.isOn = false;

			if (m_ManeuverSettingsToggle != null)
				m_ManeuverSettingsToggle.isOn = false;

			if (m_SettingsPrefab == null)
				return;

			GameObject obj = Instantiate(m_SettingsPrefab);

			if (obj == null)
				return;

			obj.transform.SetParent(transform, false);

			targetSettings = obj.GetComponent<BasicOrbit_Settings>();

			if (targetSettings == null)
				return;

			targetSettings.createSettings(basicInterface.GetTargetPanel.GetModules, "Target Panel Settings");

			//Position the panel above or below the main settings window depending on its position on screen
			RectTransform r = targetSettings.GetComponent<RectTransform>();

			if (rect.position.y - (125 * basicInterface.Scale * basicInterface.MasterScale) < 0)
			{
				float height = 223 * basicInterface.Scale * basicInterface.MasterScale;

				r.position = new Vector3(rect.position.x, rect.position.y + height, rect.position.z);
			}
			else
			{
				float y = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

				r.position = new Vector3(rect.position.x, rect.position.y - y, rect.position.z);
			}
		}

		/// <summary>
		/// Toggle to display the maneuver readout settings panel
		/// </summary>
		/// <param name="isOn">Display the maneuver panel?</param>
		public void ShowManeuverSettings(bool isOn)
		{
			//Destroy the settings panel if it is already active
			if (maneuverSettings != null)
			{
				maneuverSettings.gameObject.SetActive(false);

				Destroy(maneuverSettings);
				return;
			}

			if (!isOn)
				return;

			if (m_OrbitSettingsToggle != null)
				m_OrbitSettingsToggle.isOn = false;

			if (m_TargetSettingsToggle != null)
				m_TargetSettingsToggle.isOn = false;

			if (m_SettingsPrefab == null)
				return;

			GameObject obj = Instantiate(m_SettingsPrefab);

			if (obj == null)
				return;

			obj.transform.SetParent(transform, false);

			maneuverSettings = obj.GetComponent<BasicOrbit_Settings>();

			if (maneuverSettings == null)
				return;

			maneuverSettings.createSettings(basicInterface.GetManeuverPanel.GetModules, "Maneuver Panel Settings");

			//Position the panel above or below the main settings window depending on its position on screen
			RectTransform r = maneuverSettings.GetComponent<RectTransform>();

			if (rect.position.y - (125 * basicInterface.Scale * basicInterface.MasterScale) < 0)
			{
				float height = 142 * basicInterface.Scale * basicInterface.MasterScale;

				r.position = new Vector3(rect.position.x, rect.position.y + height, rect.position.z);
			}
			else
			{
				float y = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

				r.position = new Vector3(rect.position.x, rect.position.y - y, rect.position.z);
			}
		}

		/// <summary>
		/// Update the UI scale readout value; the scale does not update automatically with the slider
		/// </summary>
		/// <param name="scale">New scale; multiplied by ten to allow for whole number intervals on the slider</param>
		public void ApplyScale(float scale)
		{
			if (!loaded)
				return;

			if (m_ScaleText != null)
				m_ScaleText.OnTextUpdate.Invoke((scale / 10).ToString("P0"));
		}

		/// <summary>
		/// Update the readout module background transparency
		/// </summary>
		/// <param name="alpha">The new background alpha value; multiplied by 50 for the slider</param>
		public void ApplyAlpha(float alpha)
		{
			if (!loaded)
				return;

			if (m_AlphaText != null)
				m_AlphaText.OnTextUpdate.Invoke((alpha / 50).ToString("P0"));
			
			if (basicInterface == null)
				return;

			float a = alpha / 50;

			basicInterface.Alpha = a;
		}

		/// <summary>
		/// Button to manually update the UI scale for all elements
		/// </summary>
		public void SetScale()
		{
			if (m_ScaleSlider == null)
				return;

			if (basicInterface == null)
				return;

			float scale = m_ScaleSlider.value / 10;

			transform.localScale = Vector3.one * scale;

			//Updating the readout panel UI scale is handled by the upstream component of the interface module
			basicInterface.Scale = scale;
		}
	}
}
