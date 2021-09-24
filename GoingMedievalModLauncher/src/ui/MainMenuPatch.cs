using System;
using System.Reflection;
using HarmonyLib;
using NSEipix.View.UI;
using NSMedieval.UI;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GoingMedievalModLauncher.ui
{
	public static class MainMenuPatch
	{
		
		public delegate void onMenuStartPost();

		public static event onMenuStartPost OnMenuStartPost;

		public static GameObject ButtonPrefab => _buttonPrefab;

		private static GameObject _buttonPrefab = null;
		
		private static void Start()
		{

			_buttonPrefab = GameObject.Find("OptionsButton");

			if ( OnMenuStartPost == null ) return;

			OnMenuStartPost();
			OnMenuStartPost = null;
		}

		public static GameObject AddMainMenuButton(string name, string text, Action click, short pos = 0)
		{
			if ( ButtonPrefab == null )
			{
				Launcher.LOGGER.Warn("Unable to find the prefab button. Did you call this before menu initalization?");
				return null;
			}

			var button = GameObject.Instantiate(ButtonPrefab, ButtonPrefab.transform.parent);
			
			var soundB = button.GetComponent<SoundButton>();

			if ( soundB == null )
			{
				Launcher.LOGGER.Warn("Unable to find the sound button. Is this the right object?");
				Object.DestroyImmediate(button);
				return null;
			}
			
			var textO = button.transform.GetChild(0).gameObject;

			if ( textO == null )
			{
				Launcher.LOGGER.Warn("Unable to find the text child. Is this the right object?");
				Object.DestroyImmediate(button);
				return null;
			}

			var mesh = textO.GetComponent<TextMeshProUGUI>();

			if ( mesh == null )
			{
				Launcher.LOGGER.Warn("Unable to find the textmesh. Is this the right object?");
				Object.DestroyImmediate(button);
				return null;
			}

			button.name = name;
			mesh.text = text;
			button.transform.SetSiblingIndex(pos);
			soundB.PointerClickEvent += click;

			return button;

		}
		
		public static void ApplyPatch(Harmony harmony)
		{
			var orig = typeof(MainMenuView).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
			var post = typeof(MainMenuPatch).GetMethod("Start", BindingFlags.Static | BindingFlags.NonPublic);
			harmony.Patch(orig, postfix: new HarmonyMethod(post));
			
			Launcher.LOGGER.Info("MainMenuView Start was patched with postfix");
		}

	}
}