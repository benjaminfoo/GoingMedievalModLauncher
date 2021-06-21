using System;
using System.Reflection;
using HarmonyLib;
using NSMedieval.Repository;

namespace GoingMedievalModLauncher.Engine
{
	public static class PrefabRepositoryPatch
	{

		public static event Action<PrefabRepository> OnStart;

		private static void StartPost(PrefabRepository __instance)
		{
			if ( OnStart != null )
			{
				OnStart(__instance);
				OnStart = null;
			}
		}

		public static void ApplyPatch(Harmony harmony)
		{
			var orig = typeof(PrefabRepository).GetMethod(
				"Start", BindingFlags.NonPublic | BindingFlags.Instance);
			var post = typeof(PrefabRepositoryPatch).GetMethod(
				"StartPost", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(orig, new HarmonyMethod(post));
		}

	}
}