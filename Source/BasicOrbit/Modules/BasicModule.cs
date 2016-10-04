#region License
/*
 * Basic Orbit
 * 
 * BasicModule - Base class for all readout modules
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

using BasicOrbit.Unity.Interface;

namespace BasicOrbit.Modules
{
	public abstract class BasicModule : IBasicModule
	{
		private string _moduleName;
		private string _moduleValue;
		private bool _isVisible = true;
		private bool _isActive = true;
		private bool _alwaysShow;

		public BasicModule(string t)
		{
			_moduleName = t;
		}

		public string ModuleTitle
		{
			get { return _moduleName; }
		}

		public string ModuleText
		{
			get { return _moduleValue; }
		}

		public bool IsVisible
		{
			get { return _isVisible; }
			set
			{
				_isVisible = value;

				UpdateSettings();
			}
		}

		public bool AlwaysShow
		{
			get { return _alwaysShow; }
			set
			{
				_alwaysShow = value;

				UpdateSettings();
			}
		}

		protected abstract void UpdateSettings();

		public bool IsActive
		{
			get { return _isActive; }
			set { _isActive = value; }
		}

		public void Update()
		{
			_moduleValue = fieldUpdate();
		}

		protected abstract string fieldUpdate();
	}
}
