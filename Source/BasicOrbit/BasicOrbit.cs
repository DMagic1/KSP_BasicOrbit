using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using KSP.UI.Screens;
using BasicOrbit.Modules;
using BasicOrbit.Modules.OrbitModules;
using BasicOrbit.Modules.TargetModules;
using BasicOrbit.Unity.Unity;
using BasicOrbit.Unity.Interface;
using UnityEngine;

namespace BasicOrbit
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
    public class BasicOrbit : MonoBehaviour, IBasicOrbit
    {
		private BasicHUD orbitHUD;
		private BasicHUD targetHUD;

		private Apoapsis apo;
		private Periapsis peri;
		private Inclination inc;
		private Eccentricity ecc;
		private Period period;
		private LongAscending LAN;
		private RadarAltitude radar;
		private TerrainAltitude terrain;

		private ClosestApproach closest;
		private DistanceToTarget distance;
		private RelInclination relInc;
		private RelVelocity relVel;

		private BasicSettings settings;

		private static GameObject panelPrefab;

		private static BasicOrbit instance = null;

		private BasicOrbit_Panel orbitPanel;
		private BasicOrbit_Panel targetPanel;

		private BasicOrbitAppLauncher appLauncher;
		private string _version;

		public static BasicOrbit Instance
		{
			get { return instance; }
		}

		private void Awake()
		{
			if (instance != null)
				Destroy(gameObject);

			instance = this;

			settings = BasicSettings.Instance;
		}

		private void Start()
		{
			if (panelPrefab == null)
				panelPrefab = BasicOrbitLoader.Prefabs.LoadAsset<GameObject>("basicorbit_panel");

			orbitHUD = new BasicHUD(AddOrbitModules());
			orbitHUD.Position = settings.orbitPosition;

			targetHUD = new BasicHUD(AddTargetModules());
			targetHUD.Position = settings.targetPosition;

			Assembly assembly = AssemblyLoader.loadedAssemblies.GetByAssembly(Assembly.GetExecutingAssembly()).assembly;
			var ainfoV = Attribute.GetCustomAttribute(assembly, typeof(AssemblyInformationalVersionAttribute)) as AssemblyInformationalVersionAttribute;
			switch (ainfoV == null)
			{
				case true: _version = ""; break;
				default: _version = ainfoV.InformationalVersion; break;
			}

			if (settings.showOrbitPanel)
				AddOrbitPanel();

			if (settings.showTargetPanel)
				AddTargetPanel();

			appLauncher = gameObject.AddComponent<BasicOrbitAppLauncher>();
		}

		private void OnDestroy()
		{
			instance = null;

			apo.IsActive = false;
			peri.IsActive = false;
			inc.IsActive = false;
			ecc.IsActive = false;
			LAN.IsActive = false;
			period.IsActive = false;
			radar.IsActive = false;
			terrain.IsActive = false;

			closest.IsActive = false;
			distance.IsActive = false;
			relInc.IsActive = false;
			relVel.IsActive = false;

			if (orbitPanel != null)
				Destroy(orbitPanel.gameObject);

			if (targetPanel != null)
				Destroy(targetPanel.gameObject);

			settings.orbitPosition = orbitHUD.Position;
			settings.targetPosition = targetHUD.Position;

			if (appLauncher != null)
				Destroy(appLauncher);

			if (settings.Save())
				BasicOrbit.BasicLogging("Settings file saved");
		}
		
		private void Update()
		{
			if (!FlightGlobals.ready)
				return;

			if (orbitHUD == null || targetHUD == null)
				return;

			Vessel v = FlightGlobals.ActiveVessel;

			if (v == null)
				return;

			if (orbitHUD.IsVisible)
			{
				switch (v.situation)
				{
					case Vessel.Situations.LANDED:
					case Vessel.Situations.PRELAUNCH:
						apo.IsActive = false;
						peri.IsActive = false;
						inc.IsActive = false;
						ecc.IsActive = false;
						LAN.IsActive = false;
						period.IsActive = false;
						radar.IsActive = false;
						terrain.IsActive = true;
						break;
					case Vessel.Situations.SPLASHED:
						apo.IsActive = false;
						peri.IsActive = false;
						inc.IsActive = false;
						ecc.IsActive = false;
						LAN.IsActive = false;
						period.IsActive = false;
						radar.IsActive = true;
						terrain.IsActive = true;
						break;
					case Vessel.Situations.FLYING:
						apo.IsActive = true;
						radar.IsActive = true;
						terrain.IsActive = true;

						if (v.altitude > (v.mainBody.scienceValues.flyingAltitudeThreshold / 3))
							inc.IsActive = true;
						else
							inc.IsActive = false;

						LAN.IsActive = false;
						ecc.IsActive = false;
						peri.IsActive = false;
						period.IsActive = false;
						break;
					case Vessel.Situations.SUB_ORBITAL:
						apo.IsActive = true;
						radar.IsActive = true;
						inc.IsActive = true;
						ecc.IsActive = true;
						LAN.IsActive = true;

						if (v.orbit.PeR < 0)
						{
							if (Math.Abs(v.orbit.PeR) < v.mainBody.Radius / 5)
								peri.IsActive = true;
							else
								peri.IsActive = false;
						}
						else
							peri.IsActive = true;

						period.IsActive = false;
						terrain.IsActive = false;
						break;
					default:
						apo.IsActive = true;
						peri.IsActive = true;
						inc.IsActive = true;
						ecc.IsActive = true;
						LAN.IsActive = true;
						period.IsActive = true;

						radar.IsActive = false;
						terrain.IsActive = false;
						break;
				}
			}

			if (targetHUD.IsVisible)
			{
				if (!BasicTargetting.TargetValid())
				{
					closest.IsActive = false;
					distance.IsActive = false;
					relInc.IsActive = false;
					relVel.IsActive = false;

					BasicTargetting.UpdateOn = false;
				}
				else
				{
					closest.IsActive = true;
					distance.IsActive = true;
					relVel.IsActive = true;

					relInc.IsActive = (v.situation |= Vessel.Situations.DOCKED | Vessel.Situations.ESCAPING | Vessel.Situations.ORBITING | Vessel.Situations.SUB_ORBITAL) != 0;

					BasicTargetting.UpdateOn = true;
				}
			}
			else
				BasicTargetting.UpdateOn = false;

			if (BasicTargetting.UpdateOn)
				BasicTargetting.Update();
		}

		public string Version
		{
			get { return _version; }
		}

		public bool ShowOrbit
		{
			get { return settings.showOrbitPanel; }
			set
			{
				settings.showOrbitPanel = value;

				if (value)
					AddOrbitPanel();
				else
					CloseOrbit();
			}
		}

		public bool ShowTarget
		{
			get { return settings.showTargetPanel; }
			set
			{
				settings.showTargetPanel = value;

				if (value)
					AddTargetPanel();
				else
					CloseTarget();
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

		public float Scale
		{
			get { return BasicSettings.Instance.UIScale; }
			set
			{
				BasicSettings.Instance.UIScale = value;

				SetPanelScale(value);
			}
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

		public IBasicPanel GetOrbitPanel
		{
			get { return orbitHUD; }
		}

		public IBasicPanel GetTargetPanel
		{
			get { return targetHUD; }
		}

		public void ProcessStyles(GameObject obj)
		{
			BasicOrbitUtilities.processComponents(obj);
		}

		private List<IBasicModule> AddOrbitModules()
		{
			List<IBasicModule> modules = new List<IBasicModule>();

			apo = new Apoapsis("Apoapsis");
			peri = new Periapsis("Periapsis");
			inc = new Inclination("Inclination");
			ecc = new Eccentricity("Eccentricity");
			period = new Period("Period");
			LAN = new LongAscending("LAN");
			radar =new RadarAltitude("Radar Altitude");
			terrain = new TerrainAltitude("Terrain Altitude");

			apo.IsVisible = settings.showApoapsis;
			apo.AlwaysShow = settings.showApoapsisAlways;
			peri.IsVisible = settings.showPeriapsis;
			peri.AlwaysShow = settings.showPeriapsisAlways;
			inc.IsVisible = settings.showInclination;
			inc.AlwaysShow = settings.showInclinationAlways;
			ecc.IsVisible = settings.showEccentricity;
			ecc.AlwaysShow = settings.showEccentricityAlways;
			period.IsVisible = settings.showPeriod;
			period.AlwaysShow = settings.showPeriodAlways;
			LAN.IsVisible = settings.showLAN;
			LAN.AlwaysShow = settings.showLANAlways;
			radar.IsVisible = settings.showRadar;
			radar.AlwaysShow = settings.showRadarAlways;
			terrain.IsVisible = settings.showTerrain;
			terrain.AlwaysShow = settings.showTerrainAlways;

			modules.Add(apo);
			modules.Add(peri);
			modules.Add(inc);
			modules.Add(ecc);
			modules.Add(period);
			modules.Add(LAN);
			modules.Add(radar);
			modules.Add(terrain);

			return modules;
		}

		private List<IBasicModule> AddTargetModules()
		{
			List<IBasicModule> modules = new List<IBasicModule>();

			closest = new ClosestApproach("Closest Approach");
			distance = new DistanceToTarget("Dist To Target");
			relInc = new RelInclination("Rel. Inclination");
			relVel = new RelVelocity("Rel. Velocity");

			closest.IsVisible = settings.showClosestApproach;
			closest.AlwaysShow = settings.showClosestApproachAlways;
			distance.IsVisible = settings.showDistance;
			distance.AlwaysShow = settings.showDistanceAlways;
			relInc.IsVisible = settings.showRelInclination;
			relInc.AlwaysShow = settings.showRelInclinationAlways;
			relVel.IsVisible = settings.showRelVelocity;
			relVel.AlwaysShow = settings.showRelVelocityAlways;

			modules.Add(closest);
			modules.Add(distance);
			modules.Add(relInc);
			modules.Add(relVel);

			return modules;
		}

		private IEnumerator PanelStartup()
		{
			while (!FlightGlobals.ready || FlightGlobals.ActiveVessel == null)
				yield return null;

			if (settings.showOrbitPanel)
				AddOrbitPanel();

			if (settings.showTargetPanel)
				AddTargetPanel();
		}

		private void AddOrbitPanel()
		{
			if (orbitPanel != null)
				return;

			if (panelPrefab == null)
				return;

			if (orbitHUD == null)
				return;

			orbitHUD.IsVisible = true;

			GameObject obj = Instantiate(panelPrefab);

			if (obj == null)
				return;

			obj.transform.SetParent(MainCanvasUtil.MainCanvas.transform, false);

			BasicOrbitUtilities.processComponents(obj);

			orbitPanel = obj.GetComponent<BasicOrbit_Panel>();

			if (orbitPanel == null)
				return;

			orbitPanel.setPanel(orbitHUD);
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

			if (panelPrefab == null)
				return;

			if (targetHUD == null)
				return;

			targetHUD.IsVisible = true;

			GameObject obj = Instantiate(panelPrefab);

			if (obj == null)
				return;

			obj.transform.SetParent(MainCanvasUtil.MainCanvas.transform, false);

			BasicOrbitUtilities.processComponents(obj);

			targetPanel = obj.GetComponent<BasicOrbit_Panel>();

			if (targetPanel == null)
				return;

			targetPanel.setPanel(targetHUD);
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

		private void SetPanelScale(float scale)
		{
			Vector3 old = new Vector3(1, 1, 1);

			if (targetPanel != null)
				targetPanel.transform.localScale = old * scale;

			if (orbitPanel != null)
				orbitPanel.transform.localScale = old * scale;
		}

		private void SetPanelAlpha(float alpha)
		{
			if (targetPanel != null)
				targetPanel.SetAlpha(alpha);

			if (orbitPanel != null)
				orbitPanel.SetAlpha(alpha);
		}

		public static void BasicLogging(string s, params object[] m)
		{
			Debug.Log(string.Format("[Basic Orbit] " + s, m));
		}

    }
}
