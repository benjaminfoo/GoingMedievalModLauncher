using System.Reflection;
using HarmonyLib;
using NSMedieval.UI;

namespace GoingMedievalModLauncher.ui
{
	public static class MainMenuPatch
	{
		
		public delegate void onMenuStartPost();

		public static event onMenuStartPost OnMenuStartPost;
		
		private static void Start()
		{
			if ( OnMenuStartPost != null )
			{
				OnMenuStartPost();
				OnMenuStartPost = null;
			}
		}

		public static void ApplyPatch(Harmony harmony)
		{
			var orig = typeof(MainMenuView).GetMethod("Start", BindingFlags.Instance | BindingFlags.NonPublic);
			var post = typeof(MainMenuPatch).GetMethod("Start", BindingFlags.Static | BindingFlags.NonPublic);
			harmony.Patch(orig, postfix: new HarmonyMethod(post));
			
			Logger.Instance.info("MainMenuView Start was patched with postfix");
		}

	}
}