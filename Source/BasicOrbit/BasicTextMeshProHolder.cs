#region License
/*
 * Basic Orbit
 * 
 * BasicOrbitTextMeshProHolder - Extension of TextMeshProUGUI for handling dynamic text field updates
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

using BasicOrbit.Unity;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace BasicOrbit
{
	/// <summary>
	/// Extension of the TextMeshProUGUI class that allows for text field updating through Unity Events
	/// </summary>
	public class BasicOrbitTextMeshProHolder : TextMeshProUGUI
	{
		/// <summary>
		/// Reference to the text script attached to the same GameObject; attach the script either in the Unity Editor
		/// or through code at startup
		/// </summary>
		private TextHandler _handler;

		/// <summary>
		/// Get the attached TextHandler and add an event listener
		/// </summary>
		new private void Awake()
		{
			base.Awake();

			_handler = GetComponent<TextHandler>();

			if (_handler == null)
				return;

			_handler.OnTextUpdate.AddListener(new UnityAction<string>(UpdateText));
		}

		/// <summary>
		/// A Unity Event listener used to update the text field
		/// </summary>
		/// <param name="t"></param>
		private void UpdateText(string t)
		{
			text = t;
		}
	}
}