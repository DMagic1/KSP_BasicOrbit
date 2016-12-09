#region License
/*
 * Basic Orbit
 * 
 * BasicOrbitSettings - Persistent settings controller
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

using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BasicOrbit
{
	[KSPAddon(KSPAddon.Startup.MainMenu, true)]
	public class BasicSettings : MonoBehaviour
	{
		[Persistent]
		public bool showApoapsis = true;
		[Persistent]
		public bool showApoapsisAlways = false;
		[Persistent]
		public bool showPeriapsis = true;
		[Persistent]
		public bool showPeriapsisAlways = false;
		[Persistent]
		public bool showInclination = true;
		[Persistent]
		public bool showInclinationAlways = false;
		[Persistent]
		public bool showEccentricity = true;
		[Persistent]
		public bool showEccentricityAlways = false;
		[Persistent]
		public bool showPeriod = true;
		[Persistent]
		public bool showPeriodAlways = false;
		[Persistent]
		public bool showRadar = true;
		[Persistent]
		public bool showRadarAlways = false;
		[Persistent]
		public bool showOrbitAltitude = true;
		[Persistent]
		public bool showOrbitAltitudeAlways = false;
		[Persistent]
		public bool showTerrain = true;
		[Persistent]
		public bool showTerrainAlways = false;
		[Persistent]
		public bool showVelocity = true;
		[Persistent]
		public bool showVelocityAlways = false;
		[Persistent]
		public bool showLocation = true;
		[Persistent]
		public bool showLocationAlways = false;
		[Persistent]
		public bool showLAN = false;
		[Persistent]
		public bool showLANAlways = false;
		[Persistent]
		public bool showAoPe = false;
		[Persistent]
		public bool showAoPeAlways = false;
		[Persistent]
		public bool showSMA = false;
		[Persistent]
		public bool showSMAAlways = false;
		[Persistent]
		public bool showTargetName = true;
		[Persistent]
		public bool showTargetNameAlways = false;
		[Persistent]
		public bool showClosestApproach = true;
		[Persistent]
		public bool showClosestApproachAlways = false;
		[Persistent]
		public bool showClosestApproachVelocity = true;
		[Persistent]
		public bool showClosestApproachVelocityAlways = false;
		[Persistent]
		public bool showDistance = true;
		[Persistent]
		public bool showDistanceAlways = false;
		[Persistent]
		public bool showRelInclination = true;
		[Persistent]
		public bool showRelInclinationAlways = false;
		[Persistent]
		public bool showRelVelocity = true;
		[Persistent]
		public bool showRelVelocityAlways = false;
		[Persistent]
		public bool showAngleToPrograde = true;
		[Persistent]
		public bool showAngleToProgradeAlways = false;
		[Persistent]
		public bool showPhaseAngle = true;
		[Persistent]
		public bool showPhaseAngleAlways = false;
		[Persistent]
		public bool showManeuverNode = true;
		[Persistent]
		public bool showManeuverNodeAlways = false;
		[Persistent]
		public bool showManeuverBurn = true;
		[Persistent]
		public bool showManeuverBurnAlways = false;
		[Persistent]
		public bool showManeuverApoapsis = true;
		[Persistent]
		public bool showManeuverApoapsisAlways = false;
		[Persistent]
		public bool showManeuverPeriapsis = true;
		[Persistent]
		public bool showManeuverPeriapsisAlways = false;
		[Persistent]
		public bool showManeuverInclination = true;
		[Persistent]
		public bool showManeuverInclinationAlways = false;
		[Persistent]
		public bool showManeuverEccentricity = true;
		[Persistent]
		public bool showManeuverEccentricityAlways = false;
		[Persistent]
		public bool showManeuverClosestApproach = true;
		[Persistent]
		public bool showManeuverClosestApproachAlways = false;
		[Persistent]
		public bool showManeuverClosestVel = true;
		[Persistent]
		public bool showManeuverClosestVelAlways = false;
		[Persistent]
		public bool showManeuverAngleToPrograde = true;
		[Persistent]
		public bool showManeuverAngleToProgradeAlways = false;
		[Persistent]
		public bool showManeuverPhaseAngle = true;
		[Persistent]
		public bool showManeuverPhaseAngleAlways = false;
		[Persistent]
		public bool showOrbitPanel = true;
		[Persistent]
		public bool showTargetPanel = true;
		[Persistent]
		public bool showManeuverPanel = true;
		[Persistent]
		public float panelAlpha = 0.5f;
		[Persistent]
		public float UIScale = 1;
		[Persistent]
		public Vector2 orbitPosition = new Vector2(100, 100);
		[Persistent]
		public Vector2 targetPosition = new Vector2(100, 400);
		[Persistent]
		public Vector2 maneuverPosition = new Vector2(400, 100);

		private const string fileName = "PluginData/Settings.cfg";
		private string fullPath;

		private static bool loaded;
		private static BasicSettings instance;

		public static BasicSettings Instance
		{
			get { return instance; }
		}

		private void Awake()
		{
			if (loaded)
				Destroy(gameObject);

			DontDestroyOnLoad(gameObject);

			loaded = true;

			instance = this;

			fullPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName).Replace("\\", "/");

			if (Load())
				BasicOrbit.BasicLogging("Settings file loaded");
		}

		public bool Load()
		{
			bool b = false;

			try
			{
				if (File.Exists(fullPath))
				{
					ConfigNode node = ConfigNode.Load(fullPath);
					ConfigNode unwrapped = node.GetNode(GetType().Name);
					ConfigNode.LoadObjectFromConfig(this, unwrapped);
					b = true;
				}
				else
				{
					BasicOrbit.BasicLogging("Settings file could not be found [{0}]", fullPath);
					b = false;
				}
			}
			catch (Exception e)
			{
				BasicOrbit.BasicLogging("Error while loading settings file from [{0}]\n{1}", fullPath, e);
				b = false;
			}

			return b;
		}

		public bool Save()
		{
			bool b = false;

			try
			{
				ConfigNode node = AsConfigNode();
				ConfigNode wrapper = new ConfigNode(GetType().Name);
				wrapper.AddNode(node);
				wrapper.Save(fullPath);
				b = true;
			}
			catch (Exception e)
			{
				BasicOrbit.BasicLogging("Error while saving settings file from [{0}]\n{1}", fullPath, e);
				b = false;
			}

			return b;
		}

		private ConfigNode AsConfigNode()
		{
			try
			{
				ConfigNode node = new ConfigNode(GetType().Name);

				node = ConfigNode.CreateConfigFromObject(this, node);
				return node;
			}
			catch (Exception e)
			{
				BasicOrbit.BasicLogging("Failed to generate settings file node...\n{0}", e);
				return new ConfigNode(GetType().Name);
			}
		}
	}
}
