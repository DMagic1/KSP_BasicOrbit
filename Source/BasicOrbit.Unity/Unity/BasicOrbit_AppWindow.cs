#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit_AppWindow - Script for controlling the main toolbar panel
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
using System.Linq;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BasicOrbit.Unity.Unity
{
	public class BasicOrbit_AppWindow : CanvasFader, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
	{
		[SerializeField]
		private float m_MinHeight = 179;
		[SerializeField]
		private float m_MaxHeight = 560;
		[SerializeField]
		private GameObject m_OrbitBar = null;
		[SerializeField]
		private GameObject m_TargetBar = null;
		[SerializeField]
		private GameObject m_ManeuverBar = null;
		[SerializeField]
		private GameObject m_ModulePrefab = null;
		[SerializeField]
		private TextHandler m_VersionText = null;
		[SerializeField]
		private StateToggle m_OrbitDisplayToggle = null;
		[SerializeField]
		private StateToggle m_OrbitDragToggle = null;
		[SerializeField]
		private StateToggle m_TargetDisplayToggle = null;
		[SerializeField]
		private StateToggle m_TargetDragToggle = null;
		[SerializeField]
		private StateToggle m_ManeuverDisplayToggle = null;
		[SerializeField]
		private StateToggle m_ManeuverDragToggle = null;
		[SerializeField]
		private GameObject m_SettingsBar = null;
		[SerializeField]
		private TextHandler m_AlphaText = null;
		[SerializeField]
		private TextHandler m_ToolbarScaleText = null;
		[SerializeField]
		private TextHandler m_PanelScaleText = null;
		[SerializeField]
		private Slider m_AlphaSlider = null;
		[SerializeField]
		private Slider m_ToolbarScaleSlider = null;
		[SerializeField]
		private Slider m_PanelScaleSlider = null;

		private IBasicOrbit basicInterface;
		private RectTransform rect;
		private Vector2 mouseStart;
		private Vector3 windowStart;

		private bool loaded;

		protected override void Awake()
		{
			base.Awake();

			rect = GetComponent<RectTransform>();

			Alpha(0);

			Fade(1, true);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (rect == null)
				return;

			mouseStart = eventData.position;
			windowStart = rect.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (rect == null)
				return;

			rect.position = windowStart + (Vector3)(eventData.position - mouseStart);
		}

		public void OnEndDrag(PointerEventData eventData)
		{
			if (rect == null)
				return;

			if (rect == null)
				return;

			if (basicInterface == null)
				return;

			basicInterface.ClampToScreen(rect);
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			if (basicInterface != null)
				basicInterface.InMenu = true;
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			if (basicInterface != null)
				basicInterface.InMenu = false;
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			transform.SetAsLastSibling();
		}

		public void OnResize(BaseEventData eventData)
		{
			if (rect == null)
				return;

			if (!(eventData is PointerEventData))
				return;

			checkMaxResize(rect.sizeDelta.y - ((PointerEventData)eventData).delta.y);
		}

		public void OnEndResize(BaseEventData eventData)
		{
			if (!(eventData is PointerEventData))
				return;

			if (rect == null)
				return;

			checkMaxResize(rect.sizeDelta.y);

			if (basicInterface == null)
				return;

			basicInterface.Height = rect.sizeDelta.y;
		}

		private void checkMaxResize(float num)
		{
			if (rect == null)
				return;

			if (num < m_MinHeight)
				num = m_MinHeight;
			else if (num > m_MaxHeight)
				num = m_MaxHeight;

			rect.sizeDelta = new Vector2(rect.sizeDelta.x, num);
		}

		/// <summary>
		/// This method is used to initialize the panel
		/// </summary>
		/// <param name="basic">The main interface module</param>
		public void setOrbit(IBasicOrbit basic)
		{
			if (basic == null)
				return;

			basicInterface = basic;

			CreateOrbitModules(basic.GetOrbitPanel.GetModules);

			CreateTargetModules(basic.GetTargetPanel.GetModules);

			CreateManeuverModules(basic.GetManeuverPanel.GetModules);

			if (m_OrbitBar != null)
				m_OrbitBar.SetActive(false);
			
			if (m_TargetBar != null)
				m_TargetBar.SetActive(false);

			if (m_ManeuverBar != null)
				m_ManeuverBar.SetActive(false);

			if (m_OrbitDisplayToggle != null)
				m_OrbitDisplayToggle.isOn = basic.ShowOrbit;

			if (m_TargetDisplayToggle != null)
				m_TargetDisplayToggle.isOn = basic.ShowTarget;

			if (m_ManeuverDisplayToggle != null)
				m_ManeuverDisplayToggle.isOn = basic.ShowManeuver;

			if (m_AlphaText != null && m_AlphaSlider != null)
			{
				m_AlphaText.OnTextUpdate.Invoke((1 - basic.Alpha).ToString("P0"));

				m_AlphaSlider.value = (1 - basic.Alpha) * 50;
			}

			if (m_ToolbarScaleText != null && m_ToolbarScaleSlider != null)
			{
				m_ToolbarScaleText.OnTextUpdate.Invoke(basic.ToolbarScale.ToString("P0"));

				m_ToolbarScaleSlider.value = basic.ToolbarScale * 10;
			}

			if (m_PanelScaleText != null && m_PanelScaleSlider != null)
			{
				m_PanelScaleText.OnTextUpdate.Invoke(basic.PanelScale.ToString("P0"));

				m_PanelScaleSlider.value = basic.PanelScale * 10;
			}

			if (m_SettingsBar != null)
				m_SettingsBar.SetActive(false);

			if (m_VersionText != null)
				m_VersionText.OnTextUpdate.Invoke(basic.Version);

			transform.localScale = Vector3.one * basic.ToolbarScale;

			checkMaxResize(basic.Height);

			loaded = true;
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
				if (!basicInterface.GetOrbitPanel.IsVisible && m_OrbitDisplayToggle != null)
					m_OrbitDisplayToggle.isOn = true;
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
				if (!basicInterface.GetTargetPanel.IsVisible && m_TargetDisplayToggle != null)
					m_TargetDisplayToggle.isOn = true;
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
				if (!basicInterface.GetManeuverPanel.IsVisible && m_ManeuverDisplayToggle != null)
					m_ManeuverDisplayToggle.isOn = true;
			}

			basicInterface.GetManeuver.Dragging = isOn;
		}

		/// <summary>
		/// Toggle to display the orbit readout settings panel
		/// </summary>
		/// <param name="isOn">Display the settings panel?</param>
		public void ShowOrbitSettings(bool isOn)
		{
			if (m_OrbitBar != null)
				m_OrbitBar.SetActive(isOn);
		}

		/// <summary>
		/// Toggle to display the target readout settings panel
		/// </summary>
		/// <param name="isOn">Display the settings panel?</param>
		public void ShowTargetSettings(bool isOn)
		{
			if (m_TargetBar != null)
				m_TargetBar.SetActive(isOn);
		}

		/// <summary>
		/// Toggle to display the maneuver readout settings panel
		/// </summary>
		/// <param name="isOn">Display the maneuver panel?</param>
		public void ShowManeuverSettings(bool isOn)
		{
			if (m_ManeuverBar != null)
				m_ManeuverBar.SetActive(isOn);
		}

		/// <summary>
		/// Toggle to display the settings menu
		/// </summary>
		/// <param name="isOn">Display the settings menu?</param>
		public void ShowSettings(bool isOn)
		{
			if (m_SettingsBar != null)
				m_SettingsBar.SetActive(isOn);
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

			a = 1 - a;

			a = Mathf.Clamp(a, 0, 1);

			basicInterface.Alpha = a;
		}

		/// <summary>
		/// Update the UI scale readout value; the scale does not update automatically with the slider
		/// </summary>
		/// <param name="scale">New scale; multiplied by ten to allow for whole number intervals on the slider</param>
		public void ApplyToolbarScale(float scale)
		{
			if (!loaded)
				return;

			if (m_ToolbarScaleText != null)
				m_ToolbarScaleText.OnTextUpdate.Invoke((scale / 10).ToString("P0"));
		}

		/// <summary>
		/// Button to manually update the UI scale for the toolbar element
		/// </summary>
		public void SetToolbarScale()
		{
			if (m_ToolbarScaleSlider == null)
				return;

			if (basicInterface == null)
				return;

			float scale = m_ToolbarScaleSlider.value / 10;

			transform.localScale = Vector3.one * scale;

			basicInterface.ToolbarScale = scale;
		}

		/// <summary>
		/// Update the readout panel UI scale
		/// </summary>
		/// <param name="scale">New scale; multiplied by ten to allow for whole number intervals on the slider</param>
		public void ApplyPanelScale(float scale)
		{
			if (!loaded || basicInterface == null)
				return;

			if (m_PanelScaleText != null)
				m_PanelScaleText.OnTextUpdate.Invoke((scale / 10).ToString("P0"));

			basicInterface.PanelScale = scale / 10;
		}

		
		private void CreateOrbitModules(IList<IBasicModule> modules)
		{
			if (m_ModulePrefab == null || basicInterface == null || modules == null || m_OrbitBar == null)
				return;

			for (int i = modules.Count - 1; i >= 0; i--)
			{
				IBasicModule module = modules[i];

				CreateModuleSection(module, m_OrbitBar.transform);
			}
		}

		private void CreateTargetModules(IList<IBasicModule> modules)
		{
			if (m_ModulePrefab == null || basicInterface == null || modules == null || m_TargetBar == null)
				return;

			for (int i = modules.Count - 1; i >= 0; i--)
			{
				IBasicModule module = modules[i];

				CreateModuleSection(module, m_TargetBar.transform);
			}
		}

		private void CreateManeuverModules(IList<IBasicModule> modules)
		{
			if (m_ModulePrefab == null || basicInterface == null || modules == null || m_ManeuverBar == null)
				return;

			for (int i = modules.Count - 1; i >= 0; i--)
			{
				IBasicModule module = modules[i];

				CreateModuleSection(module, m_ManeuverBar.transform);
			}
		}

		private void CreateModuleSection(IBasicModule module, Transform parent)
		{
			BasicOrbit_SettingsModule element = Instantiate(m_ModulePrefab).GetComponent<BasicOrbit_SettingsModule>();

			if (element == null)
				return;

			element.transform.SetParent(parent, false);

			element.setModule(module);
		}
	}
}
