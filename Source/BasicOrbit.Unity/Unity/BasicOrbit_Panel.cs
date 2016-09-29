using System;
using System.Collections.Generic;
using System.Linq;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace BasicOrbit.Unity.Unity
{

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

		public void Close()
		{
			Fade(0, true, Kill, false);
		}

		private void Kill()
		{
			gameObject.SetActive(false);

			Destroy(gameObject);
		}

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

		public void SetOldAlpha()
		{
			if (m_Background != null)
				oldAlpha = m_Background.color.a;
		}

		public void SetAlpha(float a)
		{
			if (m_Background == null)
				return;

			Color c = m_Background.color;

			c.a = a;

			m_Background.color = c;
		}

		private void SetPosition(Vector2 v)
		{
			if (rect == null)
				return;

			rect.anchoredPosition = new Vector3(v.x, v.y > 0 ? v.y * -1 : v.y, 0);
		}

		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!dragging)
				return;

			if (rect == null)
				return;

			mouseStart = eventData.position;
			windowStart = rect.position;
		}

		public void OnDrag(PointerEventData eventData)
		{
			if (!dragging)
				return;

			if (rect == null)
				return;

			if (panelInterface == null)
				return;

			rect.position = windowStart + (Vector3)(eventData.position - mouseStart);

			rect.position = clamp(rect, new RectOffset(0, 0, 0, 0));
		}

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

		private Vector3 clamp(RectTransform r, RectOffset offset)
		{
			Vector3 pos = new Vector3();

			float f = panelInterface.Scale;

			pos.x = Mathf.Clamp(r.position.x, (-1 * (f * r.sizeDelta.x - offset.left)) - (Screen.width / 2), (Screen.width / 2) - offset.right);
			pos.y = Mathf.Clamp(r.position.y, offset.bottom - (Screen.height / 2), (Screen.height / 2) + (f * r.sizeDelta.y - offset.top));
			pos.z = 1;

			return pos;
		}

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

		private void CreateModule(IBasicModule module)
		{
			GameObject mod = Instantiate(m_ModulePrefab);

			if (mod == null)
				return;

			panelInterface.ProcessStyles(mod);

			mod.transform.SetParent(m_ModuleTransform, false);

			BasicOrbit_Module bMod = mod.GetComponent<BasicOrbit_Module>();

			if (bMod == null)
				return;

			bMod.setModule(module);

			bMod.gameObject.SetActive(module.IsVisible || module.AlwaysShow);

			Modules.Add(bMod);
		}



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
