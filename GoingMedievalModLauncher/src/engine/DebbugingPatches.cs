using System.Reflection;
using HarmonyLib;
using NSMedieval.Construction;
using NSMedieval.Pool;
using UnityEngine;

namespace GoingMedievalModLauncher.Engine
{
	public static class DebbugingPatches
	{

		private static void SpawnBlueprintPre(BuildingsManager __instance,  BuildableBase model, Vector3 worldPosition, 
		Transform transform)
		{
			Launcher.LOGGER.Info($"{model}");
			Launcher.LOGGER.Info($"{worldPosition}");
			Launcher.LOGGER.Info($"{transform}");
			var mat = transform.gameObject.GetComponentInChildren<MeshRenderer>().material;
			Launcher.LOGGER.Info($"{mat.shader.name}");
			var poolf =  typeof(BuildingsManager).GetField("pool", BindingFlags.NonPublic | BindingFlags.Instance);
			var pool = poolf.GetValue(__instance) as ObjectPool;
			var  baseBuildableView = (BaseBuildableView<BuildingInstance>) pool.Take(model.GetID());
			Launcher.LOGGER.Info($"{baseBuildableView}");
		}

		private static void AddBuildablePre(BuildingInstance instance, BaseBuildableView<BuildingInstance> view)
		{
			Launcher.LOGGER.Info($"{instance}");
			Launcher.LOGGER.Info($"{view}");
			var m = view.GetComponentsInChildren<MeshRenderer>();
			foreach ( var renderer in m )
			{
				Launcher.LOGGER.Info($"{renderer.material.shader.name}");	
			}
		}

		public static void ApplyPatches(Harmony harmony)
		{
			var orig = typeof(BuildingsManager).GetMethod(
				"SpawnBlueprint", BindingFlags.Public | BindingFlags.Instance);
			var pre = typeof(DebbugingPatches).GetMethod(
				"SpawnBlueprintPre", BindingFlags.NonPublic | BindingFlags.Static);
			var origA = typeof(BuildingsManager).GetMethod(
				"AddBuildable", BindingFlags.Public | BindingFlags.Instance);
			var preA = typeof(DebbugingPatches).GetMethod(
				"AddBuildablePre", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(orig, new HarmonyMethod(pre));
			//harmony.Patch(origA, new HarmonyMethod(preA));
			Launcher.LOGGER.Info("Debug patches were applied.");
		}

	}
}