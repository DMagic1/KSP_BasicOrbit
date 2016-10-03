using System;
using System.Collections.Generic;
using System.IO;
using BasicOrbit.Unity;
using BasicOrbit.Unity.Unity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BasicOrbit
{
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class BasicOrbitLoader : MonoBehaviour
	{
		private static bool loaded;
		private static bool TMPLoaded;
		private static bool UILoaded;

		private static GameObject[] loadedPrefabs;
		private static GameObject toolbarPrefab;
		private static GameObject panelPrefab;

		public static GameObject ToolbarPrefab
		{
			get { return toolbarPrefab; }
		}

		public static GameObject PanelPrefab
		{
			get { return panelPrefab; }
		}

		private void Awake()
		{
			if (loaded)
				Destroy(gameObject);

			if (loadedPrefabs == null)
			{
				string path = KSPUtil.ApplicationRootPath + "GameData/BasicOrbit/Resources";

				AssetBundle prefabs = AssetBundle.LoadFromFile(path + "/basic_orbit_prefabs.ksp");

				if (prefabs != null)
					loadedPrefabs = prefabs.LoadAllAssets<GameObject>();		
			}

			if (loadedPrefabs != null)
			{
				if (!TMPLoaded)
					processTMPPrefabs();

				if (UISkinManager.defaultSkin != null && !UILoaded)
					processUIPrefabs();
			}

			if (TMPLoaded && UILoaded)
				loaded = true;

			Destroy(gameObject);
		}

		private void processTMPPrefabs()
		{
			for (int i = loadedPrefabs.Length - 1; i >= 0; i--)
			{
				GameObject o = loadedPrefabs[i];

				if (o.name == "BasicOrbit_Panel")
					panelPrefab = o;
				else if (o.name == "BasicOrbit_AppLauncher")
					toolbarPrefab = o;

				if (o != null)
					processTMP(o);
			}

			TMPLoaded = true;
		}

		private void processTMP(GameObject obj)
		{
			TextHandler[] handlers = obj.GetComponentsInChildren<TextHandler>(true);

			if (handlers == null)
				return;

			for (int i = 0; i < handlers.Length; i++)
				TMProFromText(handlers[i]);
		}

		private void TMProFromText(TextHandler handler)
		{
			if (handler == null)
				return;

			Text text = handler.GetComponent<Text>();

			if (text == null)
				return;

			string t = text.text;
			Color c = text.color;
			int i = text.fontSize;
			bool r = text.raycastTarget;
			FontStyles sty = getStyle(text.fontStyle);
			TextAlignmentOptions align = getAnchor(text.alignment);
			float spacing = text.lineSpacing;
			GameObject obj = text.gameObject;

			MonoBehaviour.DestroyImmediate(text);

			BasicOrbitTextMeshProHolder tmp = obj.AddComponent<BasicOrbitTextMeshProHolder>();

			tmp.text = t;
			tmp.color = c;
			tmp.fontSize = i;
			tmp.raycastTarget = r;
			tmp.alignment = align;
			tmp.fontStyle = sty;
			tmp.lineSpacing = spacing;

			tmp.font = Resources.Load("Fonts/Calibri SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
			tmp.fontSharedMaterial = Resources.Load("Fonts/Materials/Calibri Dropshadow", typeof(Material)) as Material;

			tmp.enableWordWrapping = true;
			tmp.isOverlay = false;
			tmp.richText = true;
		}

		private FontStyles getStyle(FontStyle style)
		{
			switch (style)
			{
				case FontStyle.Normal:
					return FontStyles.Normal;
				case FontStyle.Bold:
					return FontStyles.Bold;
				case FontStyle.Italic:
					return FontStyles.Italic;
				case FontStyle.BoldAndItalic:
					return FontStyles.Bold;
				default:
					return FontStyles.Normal;
			}
		}

		private TextAlignmentOptions getAnchor(TextAnchor anchor)
		{
			switch (anchor)
			{
				case TextAnchor.UpperLeft:
					return TextAlignmentOptions.TopLeft;
				case TextAnchor.UpperCenter:
					return TextAlignmentOptions.Top;
				case TextAnchor.UpperRight:
					return TextAlignmentOptions.TopRight;
				case TextAnchor.MiddleLeft:
					return TextAlignmentOptions.MidlineLeft;
				case TextAnchor.MiddleCenter:
					return TextAlignmentOptions.Midline;
				case TextAnchor.MiddleRight:
					return TextAlignmentOptions.MidlineRight;
				case TextAnchor.LowerLeft:
					return TextAlignmentOptions.BottomLeft;
				case TextAnchor.LowerCenter:
					return TextAlignmentOptions.Bottom;
				case TextAnchor.LowerRight:
					return TextAlignmentOptions.BottomRight;
				default:
					return TextAlignmentOptions.Center;
			}
		}

		private void processUIPrefabs()
		{
			for (int i = loadedPrefabs.Length - 1; i >= 0; i--)
			{
				GameObject o = loadedPrefabs[i];

				if (o != null)
					processUIComponents(o);
			}

			UILoaded = true;
		}

		private void processUIComponents(GameObject obj)
		{
			BasicStyle[] styles = obj.GetComponentsInChildren<BasicStyle>(true);

			if (styles == null)
				return;

			for (int i = 0; i < styles.Length; i++)
				processComponents(styles[i]);
		}

		private Sprite processSliderSprites(Slider slider, bool back, ref Color color)
		{
			if (slider == null)
				return null;

			if (back)
			{
				Image background = slider.GetComponentInChildren<Image>();

				if (background == null)
					return null;

				color = background.color;

				return background.sprite;
			}
			else
			{
				RectTransform fill = slider.fillRect;

				if (fill == null)
					return null;

				Image fillImage = fill.GetComponent<Image>();

				if (fillImage == null)
					return null;

				color = fillImage.color;

				return fillImage.sprite;
			}
		}

		private BasicUIStyle getStyle(UIStyle style, UIStyleState state)
		{
			BasicUIStyle s = new BasicUIStyle();

			if (style != null)
			{
				s.Font = style.font;
				s.Style = style.fontStyle;
				s.Size = style.fontSize;
			}

			if (state != null)
			{
				s.Color = state.textColor;
			}

			return s;
		}

		private void processComponents(BasicStyle style)
		{
			if (style == null)
				return;

			UISkinDef skin = UISkinManager.defaultSkin;

			if (skin == null)
				return;

			switch (style.ElementType)
			{
				case BasicStyle.ElementTypes.Window:
					style.setImage(skin.window.normal.background, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Box:
					style.setImage(skin.box.normal.background, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Button:
					style.setButton(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.disabled.background);
					break;
				case BasicStyle.ElementTypes.Toggle:
					style.setToggle(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.disabled.background);
					break;
				case BasicStyle.ElementTypes.Slider:
					style.setSlider(skin.horizontalSlider.normal.background, skin.horizontalSliderThumb.normal.background, skin.horizontalSliderThumb.highlight.background, skin.horizontalSliderThumb.active.background, skin.horizontalSliderThumb.disabled.background);
					break;
				default:
					break;
			}
		}

	}
}
