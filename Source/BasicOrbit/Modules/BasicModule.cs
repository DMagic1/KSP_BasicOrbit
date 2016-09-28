using System;
using System.Collections.Generic;
using System.Reflection;
using BasicOrbit.Unity.Unity;
using BasicOrbit.Unity.Interface;
using UnityEngine;

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

		public void ProcessStyles(GameObject obj)
		{
			BasicOrbitUtilities.processComponents(obj);
		}

		public void Update()
		{
			_moduleValue = fieldUpdate();
		}

		protected abstract string fieldUpdate();
	}
}
