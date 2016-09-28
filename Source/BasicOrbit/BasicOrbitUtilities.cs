using KSP.UI.Screens.Flight.Dialogs;
using UnityEngine;
using UnityEngine.UI;
using BasicOrbit.Unity;
using BasicOrbit.Unity.Unity;

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
				default:
					break;
			}
		}

	}
}
