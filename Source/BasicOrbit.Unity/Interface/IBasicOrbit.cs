#region License
/*
 * Basic Orbit
 * 
 * IBasicOrbit - Interface for transferring information to and from the main toolbar UI window
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
using BasicOrbit.Unity.Unity;
using UnityEngine;

namespace BasicOrbit.Unity.Interface
{
	public interface IBasicOrbit
	{
		string Version { get; }

		bool ShowOrbit { get; set; }

		bool ShowTarget { get; set; }

		bool ShowManeuver { get; set; }

		float Alpha { get; set; }

		float Scale { get; set; }

		float MasterScale { get; }

		IBasicPanel GetOrbitPanel { get; }

		BasicOrbit_Panel GetOrbit { get; }

		IBasicPanel GetTargetPanel { get; }

		BasicOrbit_Panel GetTarget { get; }

		IBasicPanel GetManeuverPanel { get; }

		BasicOrbit_Panel GetManeuver { get; }

		void ClampToScreen(RectTransform rect);
	}
}
