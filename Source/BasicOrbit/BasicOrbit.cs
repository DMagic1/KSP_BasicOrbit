#region License
/*
 * Basic Orbit
 * 
 * BasicOrbit - Primary MonoBehaviour for controlling the addon
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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BasicOrbit.Modules.OrbitModules;
using BasicOrbit.Modules.TargetModules;
using BasicOrbit.Modules.ManeuverModules;
using BasicOrbit.Unity.Unity;
using BasicOrbit.Unity.Interface;
using UnityEngine;
using KSP.UI;

namespace BasicOrbit
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class BasicOrbit : MonoBehaviour, IBasicOrbit
    {
		private BasicHUD orbitHUD;
		private BasicHUD targetHUD;
		private BasicHUD maneuverHUD;

		private Apoapsis apo;
		private Periapsis peri;
		private Inclination inc;
		private Eccentricity ecc;
		private Period period;
		private SemiMajorAxis SMA;
		private LongAscending LAN;
		private ArgOfPeriapsis AoPE;
		private OrbitAltitude altitude;
		private RadarAltitude radar;
		private TerrainAltitude terrain;
		private Velocity velocity;
		private Location location;

		private TargetName targetName;
		private ClosestApproach closest;
		private RelVelocityAtClosest closestVel;
		private DistanceToTarget distance;
		private RelInclination relInc;
		private RelVelocity relVel;
		private AngleToPrograde angToPro;
		private PhaseAngle phaseAngle;

		private Maneuver maneuver;
		private BurnTime burnTime;
		private ManApoapsis maneuverApoapsis;
		private ManPeriapsis maneuverPeriapsis;
		private ManInclination maneuverInclination;
		private ManEccentricity maneuverEccentricity;
		private ManClosestApproach maneuverCloseApproach;
		private ManClosestRelVel maneuverCloseRelVel;
		private ManAngleToPro maneuverAngleToPro;
		private ManPhaseAngle maneuverPhaseAngle;

		private static BasicOrbit instance = null;

		private BasicOrbit_Panel orbitPanel;
		private BasicOrbit_Panel targetPanel;
		private BasicOrbit_Panel maneuverPanel;

		private BasicOrbitAppLauncher appLauncher;
		private string _version;

		private int OrbitTimer = 4;
		private int TargetTimer = 3;
		private int ManeuverTimer = 3;

		public static BasicOrbit Instance
		{
			get { return instance; }
		}

		private void Awake()
		{
			if (instance != null)
				Destroy(gameObject);

			instance = this;
		}

		private void Start()
		{
			orbitHUD = new BasicHUD(AddOrbitModules());
			orbitHUD.Position = BasicSettings.Instance.orbitPosition;

			targetHUD = new BasicHUD(AddTargetModules());
			targetHUD.Position = BasicSettings.Instance.targetPosition;

			maneuverHUD = new BasicHUD(AddManeuverModules());
			maneuverHUD.Position = BasicSettings.Instance.maneuverPosition;

			Assembly assembly = AssemblyLoader.loadedAssemblies.GetByAssembly(Assembly.GetExecutingAssembly()).assembly;
			var ainfoV = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
			switch (ainfoV == null)
			{
				case true: _version = ""; break;
				default: _version = ainfoV.InformationalVersion; break;
			}

			if (BasicSettings.Instance.showOrbitPanel)
				AddOrbitPanel();

			if (BasicSettings.Instance.showTargetPanel)
				AddTargetPanel();

			if (BasicSettings.Instance.showManeuverPanel)
				AddManeuverPanel();

			appLauncher = gameObject.AddComponent<BasicOrbitAppLauncher>();

			onGameSettings();
			GameEvents.OnGameSettingsApplied.Add(onGameSettings);
		}

		private void OnDestroy()
		{
			instance = null;

			apo.IsActive = false;
			peri.IsActive = false;
			inc.IsActive = false;
			ecc.IsActive = false;
			LAN.IsActive = false;
			AoPE.IsActive = false;
			SMA.IsActive = false;
			period.IsActive = false;
			radar.IsActive = false;
			altitude.IsActive = false;
			terrain.IsActive = false;
			velocity.IsActive = false;
			location.IsActive = false;

			targetName.IsActive = false;
			closest.IsActive = false;
			distance.IsActive = false;
			relInc.IsActive = false;
			relVel.IsActive = false;
			angToPro.IsActive = false;
			closestVel.IsActive = false;
			phaseAngle.IsActive = false;

			maneuver.IsActive = false;
			burnTime.IsActive = false;
			maneuverApoapsis.IsActive = false;
			maneuverPeriapsis.IsActive = false; 
			maneuverInclination.IsActive = false;
			maneuverEccentricity.IsActive = false;
			maneuverAngleToPro.IsActive = false;
			maneuverPhaseAngle.IsActive = false;
			maneuverCloseApproach.IsActive = false;
			maneuverCloseRelVel.IsActive = false;

			if (orbitPanel != null)
				Destroy(orbitPanel.gameObject);

			if (targetPanel != null)
				Destroy(targetPanel.gameObject);

			if (maneuverPanel != null)
				Destroy(maneuverPanel.gameObject);

			BasicSettings.Instance.orbitPosition = orbitHUD.Position;
			BasicSettings.Instance.targetPosition = targetHUD.Position;
			BasicSettings.Instance.maneuverPosition = maneuverHUD.Position;

			if (appLauncher != null)
				Destroy(appLauncher);

			if (BasicSettings.Instance.Save())
				BasicOrbit.BasicLogging("Settings file saved");

			GameEvents.OnGameSettingsApplied.Remove(onGameSettings);
		}

		private void onGameSettings()
		{
			BasicTargetting.PatchLimit = GameSettings.CONIC_PATCH_LIMIT;
		}
		
		private void Update()
		{
			if (!FlightGlobals.ready)
				return;

			if (orbitHUD == null || targetHUD == null || maneuverHUD == null)
				return;

			Vessel v = FlightGlobals.ActiveVessel;

			if (v == null)
				return;

			bool jumper = v.isEVA || v.vesselType == VesselType.Rover;
			bool highRadar = v.isEVA ? v.radarAltitude > 10 : v.radarAltitude > 5;

			if (orbitHUD.IsVisible)
			{
				if (OrbitTimer < 4)
				{
					OrbitTimer++;
				}
				else
				{
					OrbitTimer = 0;

					bool pqs = v.mainBody != null && v.mainBody.pqsController != null;

					switch (v.situation)
					{
						case Vessel.Situations.LANDED:
						case Vessel.Situations.PRELAUNCH:
							apo.IsActive = apo.AlwaysShow;
							peri.IsActive = peri.AlwaysShow;
							inc.IsActive = inc.AlwaysShow;
							ecc.IsActive = ecc.AlwaysShow;
							LAN.IsActive = LAN.AlwaysShow;
							AoPE.IsActive = AoPE.AlwaysShow;
							SMA.IsActive = SMA.AlwaysShow;
							period.IsActive = period.AlwaysShow;
							altitude.IsActive = altitude.AlwaysShow;
							radar.IsActive = radar.AlwaysShow;
							velocity.IsActive = true;
							location.IsActive = true;
							terrain.IsActive = pqs || terrain.AlwaysShow;
							break;
						case Vessel.Situations.SPLASHED:
							apo.IsActive = apo.AlwaysShow;
							peri.IsActive = peri.AlwaysShow;
							inc.IsActive = inc.AlwaysShow;
							ecc.IsActive = ecc.AlwaysShow;
							LAN.IsActive = LAN.AlwaysShow;
							AoPE.IsActive = AoPE.AlwaysShow;
							SMA.IsActive = SMA.AlwaysShow;
							period.IsActive = period.AlwaysShow;
							altitude.IsActive = altitude.AlwaysShow;
							velocity.IsActive = true;
							location.IsActive = true;
							radar.IsActive = pqs || radar.AlwaysShow;
							terrain.IsActive = pqs || terrain.AlwaysShow;
							break;
						case Vessel.Situations.FLYING:
							apo.IsActive = apo.AlwaysShow || (v.orbit.eccentricity < 1 && (!jumper || highRadar));
							radar.IsActive = radar.AlwaysShow || (pqs && (!jumper || highRadar));
							terrain.IsActive = terrain.AlwaysShow || pqs;
							location.IsActive = true;
							velocity.IsActive = true;
							inc.IsActive = inc.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 3;
							altitude.IsActive = altitude.AlwaysShow || (!pqs && (!jumper || highRadar));
							LAN.IsActive = LAN.AlwaysShow;
							AoPE.IsActive = AoPE.AlwaysShow;
							SMA.IsActive = SMA.AlwaysShow;
							ecc.IsActive = ecc.AlwaysShow;
							peri.IsActive = v.orbit.PeA > 0 && ((!jumper || highRadar));
							period.IsActive = period.AlwaysShow;
							break;
						case Vessel.Situations.SUB_ORBITAL:
							apo.IsActive = apo.AlwaysShow || (v.orbit.eccentricity < 1 && (!jumper || highRadar));
							inc.IsActive = inc.AlwaysShow || !jumper || highRadar;
							ecc.IsActive = ecc.AlwaysShow || !jumper || highRadar;
							location.IsActive = true;
							velocity.IsActive = true;

							radar.IsActive = radar.AlwaysShow || (pqs && (!jumper || highRadar));
							terrain.IsActive = terrain.AlwaysShow || pqs;
							altitude.IsActive = altitude.AlwaysShow || (!pqs && (!jumper || highRadar));

							if (v.orbit.PeA < 0)
								peri.IsActive = peri.AlwaysShow || ((Math.Abs(v.orbit.PeA) < v.mainBody.Radius || (v.orbit.eccentricity >= 1 && v.orbit.timeToPe > 0)) && (!jumper || highRadar));
							else
								peri.IsActive = peri.AlwaysShow || ((v.orbit.eccentricity < 1 || v.orbit.timeToPe > 0) && (!jumper || highRadar));

							LAN.IsActive = LAN.AlwaysShow;
							AoPE.IsActive = AoPE.AlwaysShow;
							SMA.IsActive = SMA.AlwaysShow;
							period.IsActive = period.AlwaysShow;
							break;
						default:
							apo.IsActive = apo.AlwaysShow || v.orbit.eccentricity < 1;
							peri.IsActive = peri.AlwaysShow || v.orbit.eccentricity < 1 || v.orbit.timeToPe > 0;
							inc.IsActive = true;
							ecc.IsActive = true;
							LAN.IsActive = true;
							AoPE.IsActive = true;
							SMA.IsActive = true;
							period.IsActive = period.AlwaysShow || v.orbit.eccentricity < 1;
							altitude.IsActive = true;

							velocity.IsActive = velocity.AlwaysShow;
							location.IsActive = location.AlwaysShow;
							radar.IsActive = (v.altitude < (v.mainBody.minOrbitalDistance - v.mainBody.Radius) && pqs) || radar.AlwaysShow;
							terrain.IsActive = (v.altitude < (v.mainBody.minOrbitalDistance - v.mainBody.Radius) && pqs) || terrain.AlwaysShow;
							break;
					}
				}

				if (terrain.IsActive || radar.IsActive)
					BasicOrbiting.Update();
			}

			bool targetFlag = true;

			if (targetHUD.IsVisible)
			{
				if (!BasicTargetting.TargetValid())
				{
					targetName.IsActive = false;
					closest.IsActive = false;
					distance.IsActive = false;
					relInc.IsActive = false;
					relVel.IsActive = false;
					angToPro.IsActive = false;
					closestVel.IsActive = false;
					phaseAngle.IsActive = false;

					targetFlag = false;

					BasicTargetting.UpdateOn = false;
				}
				else
				{
					if (TargetTimer < 3)
					{
						TargetTimer++;
						BasicTargetting.UpdateOn = false;
					}
					else
					{
						TargetTimer = 0;

						switch (v.situation)
						{
							case Vessel.Situations.LANDED:
							case Vessel.Situations.PRELAUNCH:
							case Vessel.Situations.SPLASHED:
								targetName.IsActive = BasicTargetting.IsCelestial || BasicTargetting.IsVessel;
								angToPro.IsActive = angToPro.AlwaysShow && BasicTargetting.ShowAngle && BasicTargetting.IsCelestial;
								phaseAngle.IsActive = phaseAngle.AlwaysShow && (BasicTargetting.ShipPhasingOrbit != null && BasicTargetting.TargetPhasingOrbit != null);
								closest.IsActive = closest.AlwaysShow && ((BasicTargetting.IsCelestial && BasicTargetting.BodyIntersect) || (BasicTargetting.IsVessel && BasicTargetting.VesselIntersect));
								closestVel.IsActive = closestVel.AlwaysShow && BasicTargetting.IsVessel && BasicTargetting.VesselIntersect;
								distance.IsActive = BasicTargetting.IsCelestial || BasicTargetting.IsVessel;
								relVel.IsActive = relVel.AlwaysShow && (BasicTargetting.IsCelestial || BasicTargetting.IsVessel);
								relInc.IsActive = relInc.AlwaysShow && (BasicTargetting.IsCelestial || BasicTargetting.IsVessel);
								break;
							case Vessel.Situations.FLYING:
								targetName.IsActive = BasicTargetting.IsCelestial || BasicTargetting.IsVessel;
								angToPro.IsActive = (angToPro.AlwaysShow && BasicTargetting.ShowAngle && BasicTargetting.IsCelestial) && (!jumper || highRadar);
								phaseAngle.IsActive = (phaseAngle.AlwaysShow && (BasicTargetting.ShipPhasingOrbit != null && BasicTargetting.TargetPhasingOrbit != null)) && (!jumper || highRadar);
								closest.IsActive = ((BasicTargetting.IsCelestial && BasicTargetting.BodyIntersect) || (BasicTargetting.IsVessel && BasicTargetting.VesselIntersect)) && (closest.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold);
								closestVel.IsActive = (BasicTargetting.IsVessel && BasicTargetting.VesselIntersect) && (closestVel.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold);
								distance.IsActive = BasicTargetting.IsCelestial || BasicTargetting.IsVessel;
								relVel.IsActive = (BasicTargetting.IsCelestial || BasicTargetting.IsVessel) && (relVel.AlwaysShow || !jumper || highRadar);
								relInc.IsActive = (BasicTargetting.IsCelestial || BasicTargetting.IsVessel) && (relInc.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 3);
								break;
							default:
								targetName.IsActive = BasicTargetting.IsCelestial || BasicTargetting.IsVessel;
								angToPro.IsActive = (BasicTargetting.ShowAngle && BasicTargetting.IsCelestial) && (angToPro.AlwaysShow || !jumper || highRadar);
								phaseAngle.IsActive = (BasicTargetting.ShipPhasingOrbit != null && BasicTargetting.TargetPhasingOrbit != null) && (phaseAngle.AlwaysShow || !jumper || highRadar);
								closest.IsActive = ((BasicTargetting.IsCelestial && BasicTargetting.BodyIntersect) || (BasicTargetting.IsVessel && BasicTargetting.VesselIntersect)) && (closest.AlwaysShow || !jumper || highRadar);
								closestVel.IsActive = (BasicTargetting.IsVessel && BasicTargetting.VesselIntersect) && (closestVel.AlwaysShow || !jumper || highRadar);
								distance.IsActive = BasicTargetting.IsCelestial || BasicTargetting.IsVessel;
								relVel.IsActive = (BasicTargetting.IsCelestial || BasicTargetting.IsVessel) && (relVel.AlwaysShow || !jumper || highRadar);
								relInc.IsActive = (BasicTargetting.IsCelestial || BasicTargetting.IsVessel) && (relInc.AlwaysShow || !jumper || highRadar);
								break;
						}

						if (AnyTargetActive())
							BasicTargetting.UpdateOn = true;
						else
							BasicTargetting.UpdateOn = false;
					}
				}
			}
			else
				BasicTargetting.UpdateOn = false;

			if (BasicTargetting.UpdateOn)
				BasicTargetting.Update();

			if (maneuverHUD.IsVisible)
			{
				if (!targetHUD.IsVisible)
				{
					if (!BasicTargetting.TargetValid())
						targetFlag = false;
				}

				PatchedConicSolver solver = v.patchedConicSolver;

				if (solver == null || solver.maneuverNodes.Count <= 0)
				{
					maneuver.IsActive = false;
					burnTime.IsActive = false;
					maneuverApoapsis.IsActive = false;
					maneuverPeriapsis.IsActive = false;
					maneuverInclination.IsActive = false;
					maneuverEccentricity.IsActive = false;
					maneuverAngleToPro.IsActive = false;
					maneuverPhaseAngle.IsActive = false;
					maneuverCloseApproach.IsActive = false;
					maneuverCloseRelVel.IsActive = false;

					BasicManeuvering.Updated = false;
					BasicManeuvering.UpdateOn = false;
				}
				else
				{
					if (ManeuverTimer < 3)
					{
						ManeuverTimer++;
						BasicManeuvering.UpdateOn = false;
					}
					else
					{
						ManeuverTimer = 0;
						switch (v.situation)
						{
							case Vessel.Situations.LANDED:
							case Vessel.Situations.PRELAUNCH:
							case Vessel.Situations.SPLASHED:
								maneuver.IsActive = maneuver.AlwaysShow;
								burnTime.IsActive = burnTime.AlwaysShow;
								maneuverApoapsis.IsActive = maneuverApoapsis.AlwaysShow;
								maneuverPeriapsis.IsActive = maneuverPeriapsis.AlwaysShow;
								maneuverInclination.IsActive = maneuverInclination.AlwaysShow;
								maneuverEccentricity.IsActive = maneuverEccentricity.AlwaysShow;
								maneuverAngleToPro.IsActive = targetFlag && maneuverAngleToPro.AlwaysShow && BasicManeuvering.ShowAngle && BasicTargetting.IsCelestial;
								maneuverPhaseAngle.IsActive = targetFlag && maneuverPhaseAngle.AlwaysShow && BasicManeuvering.PhasingNodePatch != null && BasicManeuvering.TargetPhasingOrbit != null;
								maneuverCloseApproach.IsActive = targetFlag && maneuverCloseApproach.AlwaysShow && ((BasicTargetting.IsCelestial && BasicManeuvering.BodyIntersect) || (BasicTargetting.IsVessel && BasicManeuvering.VesselIntersect));
								maneuverCloseRelVel.IsActive = targetFlag && maneuverCloseRelVel.AlwaysShow && BasicTargetting.IsVessel && BasicManeuvering.VesselIntersect;
								break;
							case Vessel.Situations.FLYING:
								maneuver.IsActive = maneuver.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2;
								burnTime.IsActive = burnTime.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2;
								maneuverApoapsis.IsActive = maneuverApoapsis.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2;
								maneuverPeriapsis.IsActive = maneuverPeriapsis.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2;
								maneuverInclination.IsActive = maneuverInclination.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2;
								maneuverEccentricity.IsActive = maneuverEccentricity.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2;
								maneuverAngleToPro.IsActive = (targetFlag && maneuverAngleToPro.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2) && BasicManeuvering.ShowAngle && BasicTargetting.IsCelestial;
								maneuverPhaseAngle.IsActive = (targetFlag && maneuverPhaseAngle.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2) && (BasicManeuvering.PhasingNodePatch != null && BasicManeuvering.TargetPhasingOrbit != null);
								maneuverCloseApproach.IsActive = targetFlag && ((BasicTargetting.IsCelestial && BasicManeuvering.BodyIntersect) || (BasicTargetting.IsVessel && BasicManeuvering.VesselIntersect)) && (maneuverCloseApproach.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2);
								maneuverCloseRelVel.IsActive = targetFlag && BasicTargetting.IsVessel && BasicManeuvering.VesselIntersect && (maneuverCloseRelVel.AlwaysShow || v.altitude > v.mainBody.scienceValues.flyingAltitudeThreshold / 2);
								break;
							default:
								Orbit nextPatch = solver.maneuverNodes[0].nextPatch;

								maneuver.IsActive = maneuver.AlwaysShow || !jumper || highRadar;
								burnTime.IsActive = burnTime.AlwaysShow || !jumper || highRadar;
								maneuverApoapsis.IsActive = maneuverApoapsis.AlwaysShow || ((nextPatch != null && nextPatch.eccentricity < 1) && (!jumper || highRadar));
								maneuverPeriapsis.IsActive = maneuverPeriapsis.AlwaysShow || ((nextPatch != null && (nextPatch.eccentricity < 1 || nextPatch.timeToPe > 0)) && (!jumper || highRadar));
								maneuverInclination.IsActive = maneuverInclination.AlwaysShow || !jumper || highRadar;
								maneuverEccentricity.IsActive = maneuverEccentricity.AlwaysShow || !jumper || highRadar;
								maneuverAngleToPro.IsActive = (targetFlag && BasicManeuvering.ShowAngle && BasicTargetting.IsCelestial) && (maneuverAngleToPro.AlwaysShow || !jumper || highRadar);
								maneuverPhaseAngle.IsActive = (targetFlag && BasicManeuvering.PhasingNodePatch != null && BasicManeuvering.TargetPhasingOrbit != null) && (maneuverPhaseAngle.AlwaysShow || !jumper || highRadar);
								maneuverCloseApproach.IsActive = (targetFlag && ((BasicTargetting.IsCelestial && BasicManeuvering.BodyIntersect) || (BasicTargetting.IsVessel && BasicManeuvering.VesselIntersect))) && (maneuverCloseApproach.AlwaysShow || !jumper || highRadar);
								maneuverCloseRelVel.IsActive = (targetFlag && BasicTargetting.IsVessel && BasicManeuvering.VesselIntersect) && (maneuverCloseRelVel.AlwaysShow || !jumper || highRadar);
								break;
						}

						if (AnyManeuverActive())
							BasicManeuvering.UpdateOn = true;
						else
							BasicManeuvering.UpdateOn = false;
					}
				}
			}
			else
				BasicManeuvering.UpdateOn = false;

			if (BasicManeuvering.UpdateOn)
				BasicManeuvering.Update(targetFlag);
		}

		public string Version
		{
			get { return _version; }
		}

		public bool ShowOrbit
		{
			get { return BasicSettings.Instance.showOrbitPanel; }
			set
			{
				BasicSettings.Instance.showOrbitPanel = value;

				if (value)
					AddOrbitPanel();
				else
					CloseOrbit();
			}
		}

		public bool ShowTarget
		{
			get { return BasicSettings.Instance.showTargetPanel; }
			set
			{
				BasicSettings.Instance.showTargetPanel = value;

				if (value)
					AddTargetPanel();
				else
					CloseTarget();
			}
		}

		public bool ShowManeuver
		{
			get { return BasicSettings.Instance.showManeuverPanel; }
			set
			{
				BasicSettings.Instance.showManeuverPanel = value;

				if (value)
					AddManeuverPanel();
				else
					CloseManeuver();
			}
		}

		public bool InMenu
		{
			get { return BasicOrbitAppLauncher.Instance == null ? false : BasicOrbitAppLauncher.Instance.InMenu; }
			set
			{
				if (BasicOrbitAppLauncher.Instance != null)
					BasicOrbitAppLauncher.Instance.InMenu = value;
			}
		}

		public float Alpha
		{
			get { return BasicSettings.Instance.panelAlpha; }
			set
			{
				BasicSettings.Instance.panelAlpha = value;

				SetPanelAlpha(value);
			}
		}

		public float ToolbarScale
		{
			get { return BasicSettings.Instance.UIScale; }
			set {BasicSettings.Instance.UIScale = value; }
		}

		public float PanelScale
		{
			get { return BasicSettings.Instance.UIPanelScale; }
			set
			{
				BasicSettings.Instance.UIPanelScale = value;

				SetPanelScale(value);
			}
		}

		public float Height
		{
			get { return BasicSettings.Instance.AppWindowHeight; }
			set { BasicSettings.Instance.AppWindowHeight = value; }
		}

		public float MasterScale
		{
			get { return GameSettings.UI_SCALE; }
		}

		public BasicOrbit_Panel GetOrbit
		{
			get { return orbitPanel; }
		}

		public BasicOrbit_Panel GetTarget
		{
			get { return targetPanel; }
		}

		public BasicOrbit_Panel GetManeuver
		{
			get { return maneuverPanel; }
		}

		public IBasicPanel GetOrbitPanel
		{
			get { return orbitHUD; }
		}

		public IBasicPanel GetTargetPanel
		{
			get { return targetHUD; }
		}

		public IBasicPanel GetManeuverPanel
		{
			get { return maneuverHUD; }
		}

		private List<IBasicModule> AddOrbitModules()
		{
			List<IBasicModule> modules = new List<IBasicModule>();

			apo = new Apoapsis("Apoapsis");
			peri = new Periapsis("Periapsis");
			inc = new Inclination("Inclination");
			ecc = new Eccentricity("Eccentricity");
			period = new Period("Period");
			SMA = new SemiMajorAxis("Semi Major Axis");
			LAN = new LongAscending("LAN");
			AoPE = new ArgOfPeriapsis("Arg of Pe");
			altitude = new OrbitAltitude("Altitude");
			radar =new RadarAltitude("Radar Altitude");
			terrain = new TerrainAltitude("Terrain Altitude");
			velocity = new Velocity("Velocity");
			location = new Location("Location");

			apo.IsVisible = BasicSettings.Instance.showApoapsis;
			apo.AlwaysShow = BasicSettings.Instance.showApoapsisAlways;
			peri.IsVisible = BasicSettings.Instance.showPeriapsis;
			peri.AlwaysShow = BasicSettings.Instance.showPeriapsisAlways;
			inc.IsVisible = BasicSettings.Instance.showInclination;
			inc.AlwaysShow = BasicSettings.Instance.showInclinationAlways;
			ecc.IsVisible = BasicSettings.Instance.showEccentricity;
			ecc.AlwaysShow = BasicSettings.Instance.showEccentricityAlways;
			period.IsVisible = BasicSettings.Instance.showPeriod;
			period.AlwaysShow = BasicSettings.Instance.showPeriodAlways;
			SMA.IsVisible = BasicSettings.Instance.showSMA;
			SMA.AlwaysShow = BasicSettings.Instance.showSMAAlways;
			LAN.IsVisible = BasicSettings.Instance.showLAN;
			LAN.AlwaysShow = BasicSettings.Instance.showLANAlways;
			AoPE.IsVisible = BasicSettings.Instance.showAoPe;
			AoPE.AlwaysShow = BasicSettings.Instance.showAoPeAlways;
			altitude.IsVisible = BasicSettings.Instance.showOrbitAltitude;
			altitude.AlwaysShow = BasicSettings.Instance.showOrbitAltitudeAlways;
			radar.IsVisible = BasicSettings.Instance.showRadar;
			radar.AlwaysShow = BasicSettings.Instance.showRadarAlways;
			terrain.IsVisible = BasicSettings.Instance.showTerrain;
			terrain.AlwaysShow = BasicSettings.Instance.showTerrainAlways;
			velocity.IsVisible = BasicSettings.Instance.showVelocity;
			velocity.AlwaysShow = BasicSettings.Instance.showVelocityAlways;
			location.IsVisible = BasicSettings.Instance.showLocation;
			location.AlwaysShow = BasicSettings.Instance.showLocationAlways;

			modules.Add(AoPE);
			modules.Add(LAN);
			modules.Add(SMA);
			modules.Add(terrain);
			modules.Add(radar);
			modules.Add(altitude);
			modules.Add(velocity);
			modules.Add(location);
			modules.Add(period);
			modules.Add(ecc);
			modules.Add(inc);
			modules.Add(peri);
			modules.Add(apo);

			return modules;
		}

		private List<IBasicModule> AddTargetModules()
		{
			List<IBasicModule> modules = new List<IBasicModule>();

			targetName = new TargetName("Target Name");
			closest = new ClosestApproach("Closest Approach");
			closestVel = new RelVelocityAtClosest("Rel Vel At Appr");
			distance = new DistanceToTarget("Dist To Target");
			relInc = new RelInclination("Rel Inclination");
			relVel = new RelVelocity("Rel Velocity");
			angToPro = new AngleToPrograde("Ang To Pro");
			phaseAngle = new PhaseAngle("Phase Angle");

			targetName.IsVisible = BasicSettings.Instance.showTargetName;
			targetName.AlwaysShow = BasicSettings.Instance.showTargetNameAlways;
			closest.IsVisible = BasicSettings.Instance.showClosestApproach;
			closest.AlwaysShow = BasicSettings.Instance.showClosestApproachAlways;
			closestVel.IsVisible = BasicSettings.Instance.showClosestApproachVelocity;
			closestVel.AlwaysShow = BasicSettings.Instance.showClosestApproachVelocityAlways;
			distance.IsVisible = BasicSettings.Instance.showDistance;
			distance.AlwaysShow = BasicSettings.Instance.showDistanceAlways;
			relInc.IsVisible = BasicSettings.Instance.showRelInclination;
			relInc.AlwaysShow = BasicSettings.Instance.showRelInclinationAlways;
			relVel.IsVisible = BasicSettings.Instance.showRelVelocity;
			relVel.AlwaysShow = BasicSettings.Instance.showRelVelocityAlways;
			angToPro.IsVisible = BasicSettings.Instance.showAngleToPrograde;
			angToPro.AlwaysShow = BasicSettings.Instance.showAngleToProgradeAlways;
			phaseAngle.IsVisible = BasicSettings.Instance.showPhaseAngle;
			phaseAngle.AlwaysShow = BasicSettings.Instance.showPhaseAngleAlways;

			modules.Add(relVel);
			modules.Add(relInc);
			modules.Add(angToPro);
			modules.Add(phaseAngle);
			modules.Add(closestVel);
			modules.Add(closest);
			modules.Add(distance);
			modules.Add(targetName);

			return modules;
		}

		private List<IBasicModule> AddManeuverModules()
		{
			List<IBasicModule> modules = new List<IBasicModule>();

			maneuver = new Maneuver("Maneuver Node");
			burnTime = new BurnTime("Burn Time");
			maneuverApoapsis = new ManApoapsis("Apoapsis");
			maneuverPeriapsis = new ManPeriapsis("Periapsis");
			maneuverInclination = new ManInclination("Inclination");
			maneuverEccentricity = new ManEccentricity("Eccentricity");
			maneuverCloseApproach = new ManClosestApproach("Closest Approach");
			maneuverCloseRelVel = new ManClosestRelVel("Rel Vel At Appr");
			maneuverAngleToPro = new ManAngleToPro("Angle To Pro");
			maneuverPhaseAngle = new ManPhaseAngle("Phase Angle");

			maneuver.IsVisible = BasicSettings.Instance.showManeuverNode;
			maneuver.AlwaysShow = BasicSettings.Instance.showManeuverNodeAlways;
			burnTime.IsVisible = BasicSettings.Instance.showManeuverBurn;
			burnTime.AlwaysShow = BasicSettings.Instance.showManeuverBurnAlways;
			maneuverApoapsis.IsVisible = BasicSettings.Instance.showManeuverApoapsis;
			maneuverApoapsis.AlwaysShow = BasicSettings.Instance.showManeuverApoapsisAlways;
			maneuverPeriapsis.IsVisible = BasicSettings.Instance.showManeuverPeriapsis;
			maneuverPeriapsis.AlwaysShow = BasicSettings.Instance.showManeuverPeriapsisAlways;
			maneuverInclination.IsVisible = BasicSettings.Instance.showManeuverInclination;
			maneuverInclination.AlwaysShow = BasicSettings.Instance.showManeuverInclinationAlways;
			maneuverEccentricity.IsVisible = BasicSettings.Instance.showManeuverEccentricity;
			maneuverEccentricity.AlwaysShow = BasicSettings.Instance.showManeuverEccentricityAlways;
			maneuverCloseApproach.IsVisible = BasicSettings.Instance.showManeuverClosestApproach;
			maneuverCloseApproach.AlwaysShow = BasicSettings.Instance.showManeuverClosestApproachAlways;
			maneuverCloseRelVel.IsVisible = BasicSettings.Instance.showManeuverClosestVel;
			maneuverCloseRelVel.AlwaysShow = BasicSettings.Instance.showManeuverClosestVelAlways;
			maneuverAngleToPro.IsVisible = BasicSettings.Instance.showManeuverAngleToPrograde;
			maneuverAngleToPro.AlwaysShow = BasicSettings.Instance.showManeuverAngleToProgradeAlways;
			maneuverPhaseAngle.IsVisible = BasicSettings.Instance.showManeuverPhaseAngle;
			maneuverPhaseAngle.AlwaysShow = BasicSettings.Instance.showManeuverPhaseAngleAlways;

			modules.Add(maneuverAngleToPro);
			modules.Add(maneuverPhaseAngle);
			modules.Add(maneuverCloseRelVel);
			modules.Add(maneuverCloseApproach);
			modules.Add(maneuverEccentricity);
			modules.Add(maneuverInclination);
			modules.Add(maneuverPeriapsis);
			modules.Add(maneuverApoapsis);
			modules.Add(burnTime);
			modules.Add(maneuver);

			return modules;
		}

		private bool AnyTargetActive()
		{
			return targetName.IsActive || closest.IsActive || closestVel.IsActive ||
				distance.IsActive || relInc.IsActive || relVel.IsActive ||
				angToPro.IsActive || phaseAngle.IsActive ||
				closest.IsVisible || closestVel.IsVisible || angToPro.IsVisible || phaseAngle.IsVisible;
		}

		private bool AnyManeuverActive()
		{
			return maneuverAngleToPro.IsActive || maneuverPhaseAngle.IsActive || maneuverCloseRelVel.IsActive ||
				maneuverCloseApproach.IsActive || maneuverEccentricity.IsActive || maneuverInclination.IsActive ||
				maneuverPeriapsis.IsActive || maneuverApoapsis.IsActive || burnTime.IsActive || maneuver.IsActive ||
				maneuverCloseApproach.IsVisible || maneuverCloseRelVel.IsVisible || maneuverAngleToPro.IsVisible || maneuverPhaseAngle.IsVisible;
		}

		private void AddOrbitPanel()
		{
			if (orbitPanel != null)
				return;

			if (BasicOrbitLoader.PanelPrefab == null)
				return;

			if (orbitHUD == null)
				return;

			orbitPanel = Instantiate(BasicOrbitLoader.PanelPrefab).GetComponent<BasicOrbit_Panel>();

			if (orbitPanel == null)
				return;

			orbitPanel.transform.SetParent(UIMasterController.Instance.dialogCanvas.transform, false);

			orbitPanel.transform.SetAsFirstSibling();

			orbitPanel.setPanel(orbitHUD);

			orbitHUD.IsVisible = true;
		}

		private void CloseOrbit()
		{
			if (orbitPanel == null)
				return;

			if (orbitHUD != null)
				orbitHUD.IsVisible = false;

			orbitPanel.Close();

			orbitPanel = null;
		}

		private void AddTargetPanel()
		{
			if (targetPanel != null)
				return;

			if (BasicOrbitLoader.PanelPrefab == null)
				return;

			if (targetHUD == null)
				return;

			targetPanel = Instantiate(BasicOrbitLoader.PanelPrefab).GetComponent<BasicOrbit_Panel>();

			if (targetPanel == null)
				return;

			targetPanel.transform.SetParent(UIMasterController.Instance.dialogCanvas.transform, false);

			targetPanel.transform.SetAsFirstSibling();

			targetPanel.setPanel(targetHUD);

			targetHUD.IsVisible = true;
		}

		private void CloseTarget()
		{
			if (targetPanel == null)
				return;

			if (targetHUD != null)
				targetHUD.IsVisible = false;

			targetPanel.Close();

			targetPanel = null;
		}

		private void AddManeuverPanel()
		{
			if (maneuverPanel != null)
				return;

			if (BasicOrbitLoader.PanelPrefab == null)
				return;

			if (maneuverHUD == null)
				return;

			maneuverPanel = Instantiate(BasicOrbitLoader.PanelPrefab).GetComponent<BasicOrbit_Panel>();

			if (maneuverPanel == null)
				return;

			maneuverPanel.transform.SetParent(UIMasterController.Instance.dialogCanvas.transform, false);

			maneuverPanel.transform.SetAsFirstSibling();

			maneuverPanel.setPanel(maneuverHUD);

			maneuverHUD.IsVisible = true;
		}

		private void CloseManeuver()
		{
			if (maneuverPanel == null)
				return;

			if (maneuverHUD != null)
				maneuverHUD.IsVisible = false;

			maneuverPanel.Close();

			maneuverPanel = null;
		}

		private void SetPanelScale(float scale)
		{
			if (targetPanel != null)
				targetPanel.transform.localScale = Vector3.one * scale;

			if (orbitPanel != null)
				orbitPanel.transform.localScale = Vector3.one * scale;

			if (maneuverPanel != null)
				maneuverPanel.transform.localScale = Vector3.one * scale;
		}

		private void SetPanelAlpha(float alpha)
		{
			if (targetPanel != null)
			{
				targetPanel.SetAlpha(alpha);
				targetPanel.SetOldAlpha();
			}

			if (orbitPanel != null)
			{
				orbitPanel.SetAlpha(alpha);
				orbitPanel.SetOldAlpha();
			}

			if (maneuverPanel != null)
			{
				maneuverPanel.SetAlpha(alpha);
				maneuverPanel.SetOldAlpha();
			}
		}

		public void ClampToScreen(RectTransform rect)
		{
			UIMasterController.ClampToScreen(rect, Vector2.zero);
		}

		public static void BasicLogging(string s, params object[] m)
		{
			Debug.Log(string.Format("[Basic_Orbit] " + s, m));
		}

    }
}
