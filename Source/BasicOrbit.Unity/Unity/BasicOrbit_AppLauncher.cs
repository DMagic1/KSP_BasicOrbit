using System;
using System.Collections.Generic;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity.Unity
{
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
		private Text m_VersionText = null;

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
				m_VersionText.text = basic.Version;

			transform.localScale = Vector3.one * basic.Scale;

			loaded = true;
		}

		public void ToggleOrbitPanel(bool isOn)
		{
			if (!loaded)
				return;

			if (basicInterface == null)
				return;

			basicInterface.ShowOrbit = isOn;

			if (!isOn && m_OrbitDragToggle != null)
				m_OrbitDragToggle.isOn = false;
		}

		public void ToggleTargetPanel(bool isOn)
		{
			if (!loaded)
				return;

			if (basicInterface == null)
				return;

			basicInterface.ShowTarget = isOn;

			if (!isOn && m_TargetDragToggle != null)
				m_TargetDragToggle.isOn = false;
		}

		public void ToggleOrbitDrag(bool isOn)
		{
			if (basicInterface == null)
				return;

			if (basicInterface.GetOrbit == null)
				return;

			if (isOn && basicInterface.GetOrbitPanel != null)
			{
				if (!basicInterface.GetOrbitPanel.IsVisible && m_OrbitToggle != null)
					m_OrbitToggle.isOn = true;
			}

			basicInterface.GetOrbit.Dragging = isOn;
		}

		public void ToggleTargetDrag(bool isOn)
		{
			if (basicInterface == null)
				return;

			if (basicInterface.GetTarget == null)
				return;

			if (isOn && basicInterface.GetTargetPanel != null)
			{
				if (!basicInterface.GetTargetPanel.IsVisible && m_TargetToggle != null)
					m_TargetToggle.isOn = true;
			}

			basicInterface.GetTarget.Dragging = isOn;
		}

		public void ShowOrbitSettings(bool isOn)
		{
			if (orbitSettings != null)
			{
				orbitSettings.gameObject.SetActive(false);

				Destroy(orbitSettings);

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

			basicInterface.ProcessStyles(obj);

			RectTransform r = orbitSettings.GetComponent<RectTransform>();

			float y = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

			if (targetSettings != null)
			{
				RectTransform tRect = targetSettings.GetComponent<RectTransform>();

				y += tRect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;
			}

			r.position = new Vector3(rect.position.x, rect.position.y - y, rect.position.z);

		}

		public void ShowTargetSettings(bool isOn)
		{
			if (targetSettings != null)
			{
				targetSettings.gameObject.SetActive(false);

				Destroy(targetSettings);

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

			basicInterface.ProcessStyles(obj);

			RectTransform r = targetSettings.GetComponent<RectTransform>();

			float y = rect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;

			if (orbitSettings != null)
			{
				RectTransform oRect = orbitSettings.GetComponent<RectTransform>();

				y += oRect.sizeDelta.y * basicInterface.Scale * basicInterface.MasterScale;
			}

			r.position = new Vector3(rect.position.x, rect.position.y - y, rect.position.z);

		}

		public void ApplyScale(float scale)
		{
			if (!loaded)
				return;

			if (m_ScaleText != null)
				m_ScaleText.text = (scale / 10).ToString("P0");
		}

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

		public void SetScale()
		{
			if (m_ScaleSlider == null)
				return;

			if (basicInterface == null)
				return;

			float scale = m_ScaleSlider.value / 10;

			transform.localScale = Vector3.one * scale;

			basicInterface.Scale = scale;
		}
	}
}
