#region License
/*
 * Basic Orbit
 * 
 * BasicOrbitAppLauncher - Stock app launcher controller
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

using System.Collections;
using BasicOrbit.Unity.Unity;
using KSP.UI.Screens;
using KSP.UI;
using UnityEngine;
using System.IO;

namespace BasicOrbit
{
    public class BasicOrbitAppLauncher : MonoBehaviour
    {
		private ApplicationLauncherButton button;
		private IEnumerator buttonAdder;

		private static BasicOrbitAppLauncher instance;
		private static Texture2D icon;

		private bool inMenu;
		private bool sticky;
		private bool hovering;

		private BasicOrbit_AppWindow launcher;

		public static BasicOrbitAppLauncher Instance
		{
			get { return instance; }
		}

		private void Start()
		{
            if (icon == null)
            {
                icon = new Texture2D(38, 38, TextureFormat.ARGB32, false);

                string path = Path.Combine(new DirectoryInfo(KSPUtil.ApplicationRootPath).FullName, "GameData/BasicOrbit/Resources/AppIcon.png").Replace("\\", "/");

                if (File.Exists(path))
                    icon.LoadImage(File.ReadAllBytes(path));
            }

			instance = this;

			if (buttonAdder != null)
				StopCoroutine(buttonAdder);

			buttonAdder = AddButton();
			StartCoroutine(buttonAdder);

			GameEvents.OnGameSettingsApplied.Add(Reposition);
		}

		private void OnDestroy()
		{
			if (launcher != null)
				Destroy(launcher.gameObject);

			GameEvents.onGUIApplicationLauncherUnreadifying.Remove(RemoveButton);
			GameEvents.OnGameSettingsApplied.Remove(Reposition);
		}

		private IEnumerator AddButton()
		{
			while (!ApplicationLauncher.Ready)
				yield return null;

			while (ApplicationLauncher.Instance == null)
				yield return null;

			button = ApplicationLauncher.Instance.AddModApplication(OnTrue, OnFalse, OnHover, OnHoverOut, null, null, ApplicationLauncher.AppScenes.FLIGHT | ApplicationLauncher.AppScenes.MAPVIEW, icon);

			ApplicationLauncher.Instance.EnableMutuallyExclusive(button);

			GameEvents.onGUIApplicationLauncherUnreadifying.Add(RemoveButton);

			buttonAdder = null;
		}

		private void RemoveButton(GameScenes scene)
		{
			if (button == null)
				return;

			ApplicationLauncher.Instance.RemoveModApplication(button);
			button = null;
		}

		private void Reposition()
		{
			if (launcher == null)
				return;

			launcher.transform.position = GetAnchor();
		}

		private void OnTrue()
		{
			sticky = true;

			Open();
		}

		private void OnFalse()
		{
			Close();
		}

		private void OnHover()
		{
			hovering = true;

			if (sticky || launcher != null)
				return;

			Open();
		}

		private void OnHoverOut()
		{
			hovering = false;

			if (!sticky)
				StartCoroutine(HoverOutWait());
		}

		private IEnumerator HoverOutWait()
		{
			int timer = 0;

			while (timer < 2)
			{
				timer++;
				yield return null;
			}

			if (! inMenu)
				Close();
		}

		private IEnumerator MenuHoverOutWait()
		{
			int timer = 0;

			while (timer < 2)
			{
				timer++;
				yield return null;
			}

			if (!hovering && !sticky)
				Close();
		}

		public bool InMenu
		{
			get { return inMenu; }
			set
			{
				inMenu = value;

				if (!value)
					StartCoroutine(MenuHoverOutWait());
			}
		}

		public Vector3 GetAnchor()
		{
			if (button == null)
				return Vector3.zero;

			Vector3 anchor = button.GetAnchor();

			anchor.x -= 3;
			anchor.y += 41;

			return anchor;
		}

		private void Open()
		{
			if (launcher != null)
				return;

			if (BasicOrbitLoader.ToolbarPrefab == null)
				return;

			launcher = (Instantiate(BasicOrbitLoader.ToolbarPrefab, GetAnchor(), Quaternion.identity) as GameObject).GetComponent<BasicOrbit_AppWindow>();

			if (launcher == null)
				return;

			launcher.transform.SetParent(UIMasterController.Instance.appCanvas.transform);

			launcher.setOrbit(BasicOrbit.Instance);
		}

		private void Close()
		{
			sticky = false;

			if (launcher == null)
				return;

			launcher.Close();

			launcher = null;
		}
    }
}
