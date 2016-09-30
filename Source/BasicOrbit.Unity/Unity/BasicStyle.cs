using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity.Unity
{
	public class BasicStyle : MonoBehaviour
	{
		public enum ElementTypes
		{
			None,
			Window,
			Box,
			Button,
			Toggle,
			Slider,
			Text
		}

		[SerializeField]
		private ElementTypes m_ElementType = ElementTypes.None;
		[SerializeField]
		private bool m_TextUpdate = false;

		public ElementTypes ElementType
		{
			get { return m_ElementType; }
		}

		public bool TextUpdate
		{
			get { return m_TextUpdate; }
		}

		private void setSelectable(BasicUIStyle style, Sprite normal, Sprite highlight, Sprite active, Sprite inactive)
		{
			//setText(style, GetComponentInChildren<Text>());

			Selectable select = GetComponent<Selectable>();

			if (select == null)
				return;

			select.image.sprite = normal;
			select.image.type = Image.Type.Sliced;
			select.transition = Selectable.Transition.SpriteSwap;

			SpriteState spriteState = select.spriteState;
			spriteState.highlightedSprite = highlight;
			spriteState.pressedSprite = active;
			spriteState.disabledSprite = inactive;
			select.spriteState = spriteState;
		}

		public void setImage(Sprite sprite, Image.Type type)
		{
			Image image = GetComponent<Image>();

			if (image == null)
				return;

			image.sprite = sprite;
			image.type = type;
		}

		public void setButton(Sprite normal, Sprite highlight, Sprite active, Sprite inactive)
		{
			setSelectable(null, normal, highlight, active, inactive);
		}

		public void setToggle(Sprite normal, Sprite highlight, Sprite active, Sprite inactive)
		{
			setSelectable(null, normal, highlight, active, inactive);

			Toggle toggle = GetComponent<Toggle>();

			if (toggle == null)
				return;

			Image toggleImage = toggle.graphic as Image;

			if (toggleImage == null)
				return;

			toggleImage.sprite = active;
			toggleImage.type = Image.Type.Sliced;
		}

		public void setSlider(Sprite background, Sprite thumb, Sprite thumbHighlight, Sprite thumbActive, Sprite thumbInactive)
		{
			setSelectable(null, thumb, thumbHighlight, thumbActive, thumbInactive);

			if (background == null)
				return;

			Slider slider = GetComponent<Slider>();

			if (slider == null)
				return;

			Image back = slider.GetComponentInChildren<Image>();

			if (back == null)
				return;

			back.sprite = background;
			back.type = Image.Type.Sliced;
		}

	}
}
