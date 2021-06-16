using System;
using System.Reflection;
using HarmonyLib;
using NSMedieval.Controllers;

namespace GoingMedievalModLauncher.Engine
{
	public static class LocalizationControllerPatch
	{

		private static void GetTextPre(ref string key)
		{
			if ( key.Contains(":") )
			{
				var modS = key.Split('#', ':');
				if ( modS.Length == 3 )
				{
					key = $"{modS[1]}:{modS[0]}{modS[2]}";
				}
			}
		}

		public static void ApplyPatch(Harmony harmony)
		{
			var orig = typeof(LocalizationController).GetMethod(
				"GetText", BindingFlags.Public | BindingFlags
					.Instance, Type.DefaultBinder, new[] { typeof(string) }, null);
			var pre = typeof(LocalizationControllerPatch).GetMethod(
				"GetTextPre", BindingFlags.NonPublic | BindingFlags.Static);

			harmony.Patch(orig, new HarmonyMethod(pre));

		}

	}
}