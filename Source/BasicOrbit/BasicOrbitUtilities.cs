using KSP.UI.Screens.Flight.Dialogs;
using UnityEngine;
using UnityEngine.UI;
using BasicOrbit.Unity;
using BasicOrbit.Unity.Unity;
using TMPro;

namespace BasicOrbit
{
	public static class BasicOrbitUtilities
	{

		public static void processComponents(GameObject obj)
		{
			if (obj == null)
				return;

			BasicStyle[] styles = obj.GetComponentsInChildren<BasicStyle>();

			if (styles == null)
				return;

			for (int i = 0; i < styles.Length; i++)
				processCompenents(styles[i]);
		}

		private static void ProcessTMPro(BasicStyle style)
		{
			Text text = style.GetComponent<Text>();

			TMProFromText(text, style.TextUpdate);
		}

		/// <summary>
		/// A method for converting a Unity Text element to a TextMeshProUGUI with the option to continuously
		/// update the new TMP object's text field
		/// </summary>
		/// <param name="text">The original Text element</param>
		/// <param name="active">A flag to make the new element continuously update its text field</param>
		private static void TMProFromText(Text text, bool active)
		{
			if (text == null)
				return;

			Transform parent = text.transform.parent;
			int index = text.transform.GetSiblingIndex();

			//Create a new GameObject to hold out TextMeshPro script
			GameObject obj = new GameObject(text.name);

			//Add the TMP script to the object
			TextMeshProUGUI tmp = obj.AddComponent<TextMeshProUGUI>();

			//Set the parent to that of the original Text object
			obj.transform.SetParent(parent, false);

			//Manually set all of the RectTransform values from the original Text object
			RectTransform rect = tmp.GetComponent<RectTransform>();
			rect.anchorMax = text.rectTransform.anchorMax;
			rect.anchorMin = text.rectTransform.anchorMin;
			rect.offsetMax = text.rectTransform.offsetMax;
			rect.offsetMin = text.rectTransform.offsetMin;
			rect.pivot = text.rectTransform.pivot;
			rect.sizeDelta = text.rectTransform.sizeDelta;

			//Set the text options based on the original Text object
			tmp.text = text.text;
			tmp.color = text.color;
			tmp.fontSize = text.fontSize;
			tmp.raycastTarget = text.raycastTarget;
			
			tmp.alignment = getAnchor(text.alignment);
			tmp.fontStyle = (FontStyles)(int)text.fontStyle;
			tmp.lineSpacing = text.lineSpacing;

			//Load fonts and materials - from MM: 
			tmp.font = Resources.Load("Fonts/Calibri SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
			tmp.fontSharedMaterial = Resources.Load("Fonts/Materials/Calibri Dropshadow", typeof(Material)) as Material;

			tmp.enableWordWrapping = false;
			tmp.isOverlay = true;
			tmp.richText = true;

			if (active)
			{
				//If we need to continuously update the text field then move the original Text object to a new MonoBehaviour
				//and use its Update to continue setting the TMP text field
				BasicTextMeshProHolder holder = BasicOrbit.Instance.gameObject.AddComponent<BasicTextMeshProHolder>();

				text.transform.SetParent(holder.transform);
				holder.Setup(tmp, text);
			}
			else
			{
				//Otherwise destroy the original Text object
				MonoBehaviour.DestroyImmediate(text);
			}
			
			//Set the new object to same position in the child heirarchy as the original Text object
			obj.transform.SetSiblingIndex(index);
		}

		/// <summary>
		/// TMP and standard Text alignment options don't match; this method converts one to the other
		/// </summary>
		/// <param name="anchor"></param>
		/// <returns></returns>
		private static TextAlignmentOptions getAnchor(TextAnchor anchor)
		{
			switch(anchor)
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

		private static Sprite processSliderSprites(Slider slider, bool back, ref Color color)
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

		private static BasicUIStyle getStyle(UIStyle style, UIStyleState state)
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

		private static void processCompenents(BasicStyle style)
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
				case BasicStyle.ElementTypes.Text:
					ProcessTMPro(style);
					break;
				default:
					break;
			}
		}

	}
}
