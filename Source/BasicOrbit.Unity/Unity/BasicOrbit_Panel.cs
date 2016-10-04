#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit_Panel - Script for controlling the readout module UI panel
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
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BasicOrbit.Unity.Unity
{
	/// <summary>
	/// This class controls the creation of the readout module panel
	/// </summary>
	public class BasicOrbit_Panel : CanvasFader, IBeginDragHandler, IDragHandler, IEndDragHandler
	{
		[SerializeField]
		private GameObject m_ModulePrefab = null;
		[SerializeField]
		private Transform m_ModuleTransform = null;
		[SerializeField]
		private Image m_Background = null;

		private Vector2 mouseStart;
		private Vector3 windowStart;

		private bool inactive;
		private RectTransform rect;
		private CanvasGroup cg;
		private IBasicPanel panelInterface;
		private List<BasicOrbit_Module> Modules = new List<BasicOrbit_Module>();
		private float oldAlpha;

		private bool dragging;

		/// <summary>
		/// Set the background alpha to full when dragging is active; return to the previous alpha when dragging is stopped
		/// </summary>
		public bool Dragging
		{
			set
			{
				dragging = value;

				if (value)
				{
					SetOldAlpha();
					SetAlpha(1);
				}
				else
					SetAlpha(oldAlpha);
			}
		}

		protected override void Awake()
		{
			base.Awake();

			rect = GetComponent<RectTransform>();
			cg = GetComponent<CanvasGroup>();

			Alpha(0);
		}

		/// <summary>
		/// The panel fades out to fully transparent when closed
		/// </summary>
		public void Close()
		{
			Fade(0, true, Kill, false);
		}

		private void Kill()
		{
			gameObject.SetActive(false);

			Destroy(gameObject);
		}

		/// <summary>
		/// This method is used to initialize the UI panel
		/// </summary>
		/// <param name="panel">The panel interface reference</param>
		public void setPanel(IBasicPanel panel)
		{
			if (panel == null)
				return;

			panelInterface = panel;

			CreateModules(panel.GetModules);

			SetPosition(panel.Position);

			transform.localScale *= panel.Scale;

			Fade(1, true);

			SetAlpha(panel.Alpha);

			SetOldAlpha();
		}

		/// <summary>
		/// Cached the background image alpha when dragging is active
		/// </summary>
		public void SetOldAlpha()
		{
			if (m_Background != null)
				oldAlpha = m_Background.color.a;
		}

		/// <summary>
		/// Set a new background image alpha; used when dragging or when changing the background transparency from the settings panel
		/// </summary>
		/// <param name="a">The new background alpha value</param>
		public void SetAlpha(float a)
		{
			if (m_Background == null)
				return;

			Color c = m_Background.color;

			c.a = a;

			m_Background.color = c;
		}

		/// <summary>
		/// Sets the panel position
		/// </summary>
		/// <param name="v">The x and y coordinates of the panel, measured from the top-left</param>
		private void SetPosition(Vector2 v)
		{
			if (rect == null)
				return;

			rect.anchoredPosition = new Vector3(v.x, v.y > 0 ? v.y * -1 : v.y, 0);
		}

		/// <summary>
		/// Interface method to begin drag operation
		/// </summary>
		/// <param name="eventData"></param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!dragging)
				return;

			if (rect == null)
				return;

			mouseStart = eventData.position;
			windowStart = rect.position;
		}

		/// <summary>
		/// Interface method to update the panel position on drag
		/// </summary>
		/// <param name="eventData"></param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!dragging)
				return;

			if (rect == null)
				return;

			if (panelInterface == null)
				return;

			rect.position = windowStart + (Vector3)(eventData.position - mouseStart);
		}

		/// <summary>
		/// Interface method to end drag operation and clamp the panel to the screen
		/// </summary>
		/// <param name="eventData"></param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!dragging)
				return;

			if (rect == null)
				return;

			if (panelInterface == null)
				return;

			panelInterface.ClampToScreen(rect);

			panelInterface.Position = new Vector2(rect.anchoredPosition.x, rect.anchoredPosition.y);
		}

		/// <summary>
		/// Create the individual readout modules for the panel
		/// </summary>
		/// <param name="modules">The list of available readout modules for this panel</param>
		private void CreateModules(IList<IBasicModule> modules)
		{
			if (modules == null)
				return;

			if (panelInterface == null)
				return;

			if (m_ModulePrefab == null || m_ModuleTransform == null)
				return;

			for (int i = modules.Count - 1; i >= 0; i--)
			{
				IBasicModule module = modules[i];

				if (module == null)
					continue;

				CreateModule(module);
			}
		}

		/// <summary>
		/// Create the individual readout module using the Readout Module prefab
		/// </summary>
		/// <param name="module">The readout module interface</param>
		private void CreateModule(IBasicModule module)
		{
			GameObject mod = Instantiate(m_ModulePrefab);

			if (mod == null)
				return;

			mod.transform.SetParent(m_ModuleTransform, false);

			BasicOrbit_Module bMod = mod.GetComponent<BasicOrbit_Module>();

			if (bMod == null)
				return;

			bMod.setModule(module);

			bMod.gameObject.SetActive(module.IsVisible || module.AlwaysShow);

			Modules.Add(bMod);
		}

		/// <summary>
		/// Update panel visibility based on the status of each readout module
		/// Update any active modules found
		/// </summary>
		private void Update()
		{
			if (panelInterface == null)
				return;

			if (!panelInterface.IsVisible)
				return;

			if (panelInterface.AnyActive || dragging)
			{
				if (inactive)
				{
					inactive = false;

					Fade(1, false);

					if (cg != null)
					{
						cg.interactable = true;
						cg.blocksRaycasts = true;
					}
				}
			}
			else
			{
				if (!inactive)
				{
					inactive = true;

					Fade(0, false);

					if (cg != null)
					{
						cg.interactable = false;
						cg.blocksRaycasts = false;
					}
				}
				return;
			}

			for (int i = Modules.Count - 1; i >= 0; i--)
			{
				BasicOrbit_Module mod = Modules[i];

				if (mod == null)
					continue;

				if (!mod.IsVisible)
				{
					if (mod.gameObject.activeSelf)
						mod.gameObject.SetActive(false);

					continue;
				}

				if (mod.AlwaysShow)
				{
					if (!mod.gameObject.activeSelf)
						mod.gameObject.SetActive(true);

					mod.UpdateModule();
					continue;
				}

				if (mod.IsActive)
				{
					if (!mod.gameObject.activeSelf)
						mod.gameObject.SetActive(true);

					mod.UpdateModule();
				}
				else if (mod.gameObject.activeSelf)
					mod.gameObject.SetActive(false);
			}
		}

	}
}
