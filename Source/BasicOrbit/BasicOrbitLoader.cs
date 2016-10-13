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

using BasicOrbit.Unity;
using BasicOrbit.Unity.Unity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace BasicOrbit
{
	/// <summary>
	/// KSPAddon script for processing the Unity UI prefabs, replacing Text elements with TextMeshPro elements, and updating the UI styling with KSP styles
	/// </summary>
	[KSPAddon(KSPAddon.Startup.EveryScene, false)]
	public class BasicOrbitLoader : MonoBehaviour
	{
		private static bool loaded;
		private static bool TMPLoaded;
		private static bool UILoaded;

		private static GameObject[] loadedPrefabs;

		//Cache prefabs used by the KSP side of the program
		private static GameObject toolbarPrefab;
		private static GameObject panelPrefab;

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
				Destroy(gameObject);

			//Load the prefab assetbundle immediately upon startup
			if (loadedPrefabs == null)
			{
				string path = KSPUtil.ApplicationRootPath + "GameData/BasicOrbit/Resources";

				AssetBundle prefabs = AssetBundle.LoadFromFile(path + "/basic_orbit_prefabs");

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
				else if (o.name == "BasicOrbit_AppLauncher")
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
			FontStyles sty = getStyle(text.fontStyle);
			TextAlignmentOptions align = getAnchor(text.alignment);
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
			tmp.font = Resources.Load("Fonts/Calibri SDF", typeof(TMP_FontAsset)) as TMP_FontAsset;
			tmp.fontSharedMaterial = Resources.Load("Fonts/Materials/Calibri Dropshadow", typeof(Material)) as Material;

			tmp.enableWordWrapping = true;
			tmp.isOverlay = false;
			tmp.richText = true;
		}

		/// <summary>
		/// TMP FontStyles don't match up with the standard Unity FontStyle; there is no BoldAndItalic and TMP has many more styles
		/// </summary>
		/// <param name="style">The standard Unity FontStyle</param>
		/// <returns>The corresponding TMP FontStyles</returns>
		private FontStyles getStyle(FontStyle style)
		{
			switch (style)
			{
				case FontStyle.Normal:
					return FontStyles.Normal;
				case FontStyle.Bold:
					return FontStyles.Bold;
				case FontStyle.Italic:
					return FontStyles.Italic;
				case FontStyle.BoldAndItalic:
					return FontStyles.Bold;
				default:
					return FontStyles.Normal;
			}
		}

		/// <summary>
		/// TMP has many more text aligment options; convert to them here
		/// </summary>
		/// <param name="anchor">The standard Unity TextAnchor</param>
		/// <returns>The corresponding TMP TextAlignmentOptions</returns>
		private TextAlignmentOptions getAnchor(TextAnchor anchor)
		{
			switch (anchor)
			{
				case TextAnchor.UpperLeft:
					return TextAlignmentOptions.TopLeft;
				case TextAnchor.UpperCenter:
					return TextAlignmentOptions.Top;
				case TextAnchor.UpperRight:
					return TextAlignmentOptions.TopRight;
				case TextAnchor.MiddleLeft:
					return TextAlignmentOptions.MidlineLeft;
				case TextAnchor.MiddleCenter:
					return TextAlignmentOptions.Midline;
				case TextAnchor.MiddleRight:
					return TextAlignmentOptions.MidlineRight;
				case TextAnchor.LowerLeft:
					return TextAlignmentOptions.BottomLeft;
				case TextAnchor.LowerCenter:
					return TextAlignmentOptions.Bottom;
				case TextAnchor.LowerRight:
					return TextAlignmentOptions.BottomRight;
				default:
					return TextAlignmentOptions.Center;
			}
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
					style.setImage(skin.window.normal.background, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Box:
					style.setImage(skin.box.normal.background, Image.Type.Sliced);
					break;
				case BasicStyle.ElementTypes.Button:
					style.setButton(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.disabled.background);
					break;
				case BasicStyle.ElementTypes.Toggle:
					style.setToggle(skin.button.normal.background, skin.button.highlight.background, skin.button.active.background, skin.button.disabled.background);
					break;
				case BasicStyle.ElementTypes.Slider:
					style.setSlider(skin.horizontalSlider.normal.background, skin.horizontalSliderThumb.normal.background, skin.horizontalSliderThumb.highlight.background, skin.horizontalSliderThumb.active.background, skin.horizontalSliderThumb.disabled.background);
					break;
				default:
					break;
			}
		}

	}
}
