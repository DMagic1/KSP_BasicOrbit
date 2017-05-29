#region License
/*
 * Basic Orbit
 * 
 * BasicStyle - Script for controlling the selection of UI style elements
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

using UnityEngine;
using UnityEngine.UI;

namespace BasicOrbit.Unity.Unity
{
	/// <summary>
	/// This script is attached to UI elements to apply UI styling to them
	/// </summary>
	public class BasicStyle : MonoBehaviour
	{
		/// <summary>
		/// The available categories of UI element types
		/// </summary>
		public enum ElementTypes
		{
			None,
			Window,
			Box,
			Button,
			Toggle,
			Slider,
			Header,
			Footer,
			Content,
			ContentToggle,
			ContentFooter
		}

		[SerializeField]
		private ElementTypes m_ElementType = ElementTypes.None;

		public ElementTypes ElementType
		{
			get { return m_ElementType; }
		}

		/// <summary>
		/// Sets the sprites for any Selectable UI element; button, toggle, slider
		/// </summary>
		/// <param name="style"></param>
		/// <param name="normal">The normal sprite</param>
		/// <param name="highlight">Sprite when mouse is over the UI element</param>
		/// <param name="active">Sprite when mouse is clicking on the UI element</param>
		/// <param name="inactive">Sprite when the element is disabled; not generally used</param>
		private void setSelectable(Sprite normal, Sprite highlight, Sprite active, Sprite inactive)
		{
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

		private void setSelectable(Sprite normal)
		{
			Selectable select = GetComponent<Selectable>();

			if (select == null)
				return;

			select.image.sprite = normal;
			select.image.type = Image.Type.Sliced;
			select.transition = Selectable.Transition.None;
		}

		/// <summary>
		/// Sets the background image for windows and panels
		/// </summary>
		/// <param name="sprite">The background sprite</param>
		/// <param name="type">Sprite type; generally sliced</param>
		public void setImage(Sprite sprite, Image.Type type)
		{
			Image image = GetComponent<Image>();

			if (image == null)
				return;

			image.sprite = sprite;
			image.type = type;
		}

		/// <summary>
		/// Setup UI stlye for buttons
		/// </summary>
		/// <param name="normal">The normal sprite</param>
		/// <param name="highlight">Sprite when mouse is over the UI element</param>
		/// <param name="active">Sprite when mouse is clicking on the UI element</param>
		/// <param name="inactive">Sprite when the element is disabled; not generally used</param>
		public void setButton(Sprite normal, Sprite highlight, Sprite active, Sprite inactive)
		{
			setSelectable(normal, highlight, active, inactive);
		}

		/// <summary>
		/// Setup UI style for toggles
		/// </summary>
		/// <param name="normal">The normal checkbox sprite</param>
		/// <param name="onMark">Sprite for the on state</param>
		/// <param name="offMark">Sprite for the off state</param>
		public void setToggle(Sprite normal, Sprite onMark, Sprite offMark)
		{
			setSelectable(normal);

			if (onMark == null || offMark == null)
				return;

			StateToggle toggle = GetComponent<StateToggle>();

			if (toggle == null)
				return;

			toggle.OnSprite = onMark;
			toggle.OffSprite = offMark;
		}

		/// <summary>
		/// Setup the UI style for sliders
		/// </summary>
		/// <param name="background">The slider background sprite</param>
		/// <param name="thumb">The slider thumb normal sprite</param>
		/// <param name="thumbHighlight">Sprite when mouse is over the slider thumb</param>
		/// <param name="thumbActive">Sprite when mouse is clicking on the slider thumb</param>
		/// <param name="thumbInactive">Sprite when the element is disabled; not generally used</param>
		public void setSlider(Sprite background, Sprite thumb, Sprite thumbHighlight, Sprite thumbActive, Sprite thumbInactive)
		{
			//The slider thumb is the selectable component
			setSelectable(thumb, thumbHighlight, thumbActive, thumbInactive);

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
