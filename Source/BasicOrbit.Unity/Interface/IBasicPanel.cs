#region License
/*
 * Basic Orbit
 * 
 * IBasicPanel - Interface for transferring information to and from the readout panel UI element
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
using UnityEngine;

namespace BasicOrbit.Unity.Interface
{
	public interface IBasicPanel
	{
		bool IsVisible { get; set; }

		bool AnyActive { get; }

		float Alpha { get; }

		float Scale { get; }
		
		Vector2 Position { get; set; }

		IList<IBasicModule> GetModules { get; }

		void ClampToScreen(RectTransform rect);
	}
}
