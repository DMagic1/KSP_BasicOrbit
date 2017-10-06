#region License
/*
 * Basic Orbit
 * 
 * BasicOrbitLoader - MonoBehaviour for final processing of the Unity prefabs
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
using System.Linq;
using BasicOrbit.Unity;
using BasicOrbit.Unity.Unity;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using KSP.UI;
using KSP.UI.Screens;

namespace BasicOrbit
{
	/// <summary>
	/// KSPAddon script for processing the Unity UI prefabs, replacing Text elements with TextMeshPro elements, and updating the UI styling with KSP styles
	/// </summary>
	[KSPAddon(KSPAddon.Startup.Flight, true)]
	public class BasicOrbitLoader : MonoBehaviour
	{
		private const string bundleName = "/basic_orbit_prefabs";
		private const string bundlePath = "GameData/BasicOrbit/Resources";

		private static bool loaded;
		private static bool TMPLoaded;
		private static bool UILoaded;
		private static bool spritesLoaded;

		private static GameObject[] loadedPrefabs;

		//Cache prefabs used by the KSP side of the program
		private static GameObject toolbarPrefab;
		private static GameObject panelPrefab;

		private static Sprite titleSprite;
		private static Sprite footerSprite;
		private static Sprite contentFooterSprite;
		private static Sprite buttonSprite;
		private static Sprite componentSprite;
		private static Sprite selectedSprite;
		private static Sprite unselectedSprite;
		private static Sprite windowSprite;
		private static Sprite panelSprite;

		public static GameObject ToolbarPrefab
		{
			get { return toolbarPrefab; }
		}

		public static GameObject PanelPrefab
		{
			get { return panelPrefab; }
		}

		private void Awake()
		{
			//Destroy the script once everything is processed
			if (loaded)
			{
				Destroy(gameObject);
				return;
			}

			if (!spritesLoaded)
				loadSprites();

			//Load the prefab assetbundle immediately upon startup
			if (loadedPrefabs == null)
			{
				string path = KSPUtil.ApplicationRootPath + bundlePath;

				AssetBundle prefabs = AssetBundle.LoadFromFile(path + bundleName);

				if (prefabs != null)
					loadedPrefabs = prefabs.LoadAllAssets<GameObject>();		
			}

			if (loadedPrefabs != null)
			{
				if (!TMPLoaded)
					processTMPPrefabs();

				//The UISkinManager is only active once the MainMenu is reached
				if (UISkinManager.defaultSkin != null && !UILoaded)
					processUIPrefabs();
			}

			if (TMPLoaded && UILoaded)
				loaded = true;

			Destroy(gameObject);
		}

		private void loadSprites()
		{
			ContractsApp prefab = null;

			var prefabs = Resources.FindObjectsOfTypeAll<ContractsApp>();

			for (int i = prefabs.Length - 1; i >= 0; i--)
			{
				var pre = prefabs[i];

				if (pre.name != "ContractsApp")
					continue;

				prefab = pre;
				break;
			}

			if (prefab != null)
			{

				GenericAppFrame appFrame = null;
				GenericCascadingList cascadingList = null;
				UIListItem_spacer spacer = null;

				try
				{
					var fields = typeof(ContractsApp).GetFields(BindingFlags.NonPublic | BindingFlags.Instance).ToArray();

					appFrame = fields[7].GetValue(prefab) as GenericAppFrame;

					cascadingList = fields[9].GetValue(prefab) as GenericCascadingList;

					spacer = fields[11].GetValue(prefab) as UIListItem_spacer;
				}
				catch (Exception e)
				{
					BasicOrbit.BasicLogging("Error in processing toolbar panel UI: {0}", e);
				}

				if (appFrame != null)
				{
					windowSprite = appFrame.gfxBg.sprite;
					titleSprite = appFrame.gfxHeader.sprite;
					footerSprite = appFrame.gfxFooter.sprite;
				}

				if (cascadingList != null)
				{
					buttonSprite = cascadingList.cascadeHeader.GetComponent<Image>().sprite;
					contentFooterSprite = cascadingList.cascadeFooter.GetComponent<Image>().sprite;
				}

				if (spacer != null)
				{
					componentSprite = spacer.GetComponent<Image>().sprite;

					UIStateImage stateImage = spacer.GetComponentInChildren<UIStateImage>();

					selectedSprite = stateImage.states[1].sprite;
					unselectedSprite = stateImage.states[0].sprite;
				}
			}

			StageManager prefabFlight = null;

			var stages = Resources.FindObjectsOfTypeAll<StageManager>();

			for (int i = stages.Length - 1; i >= 0; i--)
			{
				var pre = stages[i];

				if (pre.name == "StageManager")
					prefabFlight = pre;
			}

			if (prefabFlight != null)
			{
				StageGroup group = prefabFlight.stageGroupPrefab;

				Transform layout = group.transform.FindChild("IconLayout");

				if (layout != null)
					panelSprite = layout.GetComponent<Image>().sprite;
			}

			spritesLoaded = true;
		}

		/// <summary>
		/// This method is used to parse all the loaded UI prefab elements and to cache certain prefabs
		/// </summary>
		private void processTMPPrefabs()
		{
			for (int i = loadedPrefabs.Length - 1; i >= 0; i--)
			{
				GameObject o = loadedPrefabs[i];

				if (o.name == "BasicOrbit_Panel")
					panelPrefab = o;
				else if (o.name == "BasicOrbit_AppWindow")
					toolbarPrefab = o;

				if (o != null)
					processTMP(o);
			}

			TMPLoaded = true;
		}

		/// <summary>
		/// This method searches for all objects with the TextHandler script attached and processes them
		/// </summary>
		/// <param name="obj">The prefab object</param>
		private void processTMP(GameObject obj)
		{
			TextHandler[] handlers = obj.GetComponentsInChildren<TextHandler>(true);

			if (handlers == null)
				return;

			for (int i = 0; i < handlers.Length; i++)
				TMProFromText(handlers[i]);
		}

		/// <summary>
		/// This method replaces standard Unity Text elements with TextMeshPro elements
		/// </summary>
		/// <param name="handler">The text element marker</param>
		private void TMProFromText(TextHandler handler)
		{
			if (handler == null)
				return;

			//The TextHandler element should be attached only to objects with a Unity Text element
			//Note that the "[RequireComponent(typeof(Text))]" attribute cannot be attached to TextHandler since Unity will not allow the Text element to be removed
			Text text = handler.GetComponent<Text>();

			if (text == null)
				return;

			//Cached all of the relevent information from the Text element
			string t = text.text;
			Color c = text.color;
			int i = text.fontSize;
			bool r = text.raycastTarget;
			FontStyles sty = TMPProUtil.FontStyle(text.fontStyle);
			TextAlignmentOptions align = TMPProUtil.TextAlignment(text.alignment);
			float spacing = text.lineSpacing;
			GameObject obj = text.gameObject;

			//The existing Text element must by destroyed since Unity will not allow two UI elements to be placed on the same GameObject
			MonoBehaviour.DestroyImmediate(text);

			BasicOrbitTextMeshProHolder tmp = obj.AddComponent<BasicOrbitTextMeshProHolder>();

			//Populate the TextMeshPro fields with the cached data from the old Text element
			tmp.text = t;
			tmp.color = c;
			tmp.fontSize = i;
			tmp.raycastTarget = r;
			tmp.alignment = align;
			tmp.fontStyle = sty;
			tmp.lineSpacing = spacing;

			//Load the TMP Font from disk
			tmp.font = UISkinManager.TMPFont;
			tmp.fontSharedMaterial = Resources.Load("Fonts/Materials/Calibri Dropshadow", typeof(Material)) as Material;

			tmp.enableWordWrapping = true;
			tmp.isOverlay = false;
			tmp.richText = true;
		}

		/// <summary>
		/// This method is used to parse all prefab objects
		/// </summary>
		private void processUIPrefabs()
		{
			for (int i = loadedPrefabs.Length - 1; i >= 0; i--)
			{
				GameObject o = loadedPrefabs[i];

				if (o != null)
					processUIComponents(o);
			}

			UILoaded = true;
		}

		/// <summary>
		/// This method searches for UI elements with the BasicStyle tag
		/// </summary>
		/// <param name="obj">The prefab object</param>
		private void processUIComponents(GameObject obj)
		{
			BasicStyle[] styles = obj.GetComponentsInChildren<BasicStyle>(true);

			if (styles == null)
				return;

			for (int i = 0; i < styles.Length; i++)
				processComponents(styles[i]);
		}

		/// <summary>
		/// Fill in the UI style elements using the default KSP skin elements
		/// </summary>
		/// <param name="style"></param>
		private void processComponents(BasicStyle style)
		{
			if (style == null)
				return;

			UISkinDef skin = UISkinManager.defaultSkin;

			if (skin == null)
				return;

			switch (style.ElementType)
			{
				case BasicStyle.ElementTypes.Window:
					style.setImage(panelSprite, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Box:
					style.setImage(windowSprite, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Button:
					style.setButton(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.disabled.background);
					break;
				case BasicStyle.ElementTypes.Toggle:
					style.setToggle(buttonSprite, null, null);
					break;
				case BasicStyle.ElementTypes.Slider:
					style.setSlider(skin.horizontalSlider.normal.background, skin.horizontalSliderThumb.normal.background, skin.horizontalSliderThumb.highlight.background, skin.horizontalSliderThumb.active.background, skin.horizontalSliderThumb.disabled.background);
					break;
				case BasicStyle.ElementTypes.Header:
					style.setImage(titleSprite, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Footer:
					style.setImage(footerSprite, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Content:
					style.setImage(componentSprite, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.ContentFooter:
					style.setImage(contentFooterSprite, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.ContentToggle:
					style.setToggle(componentSprite, selectedSprite, unselectedSprite);
					break;
				default:
					break;
			}
		}

	}
}
