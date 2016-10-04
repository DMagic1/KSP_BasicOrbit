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
		private Toggle m_OrbitDragToggle = null;
		[SerializeField]
		private Toggle m_TargetDragToggle = null;
		[SerializeField]
		private Text m_AlphaText = null;
		[SerializeField]
		private Text m_ScaleText = null;
		[SerializeField]
		private Slider m_AlphaSlider = null;
		[SerializeField]
		private Slider m_ScaleSlider = null;
		[SerializeField]
		private TextHandler m_VersionText = null;

		private BasicOrbit_Settings orbitSettings;
		private BasicOrbit_Settings targetSettings;

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

			if (m_AlphaText != null && m_AlphaSlider != null)
			{
				m_AlphaText.text = basic.Alpha.ToString("P0");

				m_AlphaSlider.value = basic.Alpha * 50;
			}

			if (m_ScaleText != null && m_ScaleSlider != null)
			{
				m_ScaleText.text = basic.Scale.ToString("P0");

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

				//If the other settings panel is open update its position
				if (targetSettings != null)
				{
					RectTransform r1 = targetSettings.GetComponent<RectTransform>();

					float y1 = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

					r1.position = new Vector3(rect.position.x, rect.position.y - y1, rect.position.z);
				}
			}

			if (!isOn)
				return;

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

			//Position the panel below the main settings window and below any other active settings panels
			RectTransform r = orbitSettings.GetComponent<RectTransform>();

			float y = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

			if (targetSettings != null)
			{
				RectTransform tRect = targetSettings.GetComponent<RectTransform>();

				y += tRect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;
			}

			r.position = new Vector3(rect.position.x, rect.position.y - y, rect.position.z);

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

				//If the other settings panel is open update its position
				if (orbitSettings != null)
				{
					RectTransform r1 = orbitSettings.GetComponent<RectTransform>();

					float y1 = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

					r1.position = new Vector3(rect.position.x, rect.position.y - y1, rect.position.z);
				}
			}

			if (!isOn)
				return;

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

			//Position the panel below the main settings window and below any other active settings panels
			RectTransform r = targetSettings.GetComponent<RectTransform>();

			float y = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

			if (orbitSettings != null)
			{
				RectTransform oRect = orbitSettings.GetComponent<RectTransform>();

				y += oRect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;
			}

			r.position = new Vector3(rect.position.x, rect.position.y - y, rect.position.z);

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
				m_ScaleText.text = (scale / 10).ToString("P0");
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
				m_AlphaText.text = (alpha / 50).ToString("P0");
			
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
