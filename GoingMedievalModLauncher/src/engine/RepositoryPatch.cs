using System.Reflection;
using HarmonyLib;
using NSEipix.Base;
using NSEipix.Repository;
using NSMedieval.Production;
using UnityEngine;

namespace GoingMedievalModLauncher.Engine
{
	public static class RepositoryPatch<T, M> where T : Repository<T, M> where M : NSEipix.Base.Model
	{

		public delegate void onAction(T repo);
		
		public static event onAction PostAwake;
		public static event onAction PostDeserialization;


		private static void Awake(MonoSingleton<T> __instance)
		{
			if ( __instance is T repo && PostAwake != null)
			{
				PostAwake(repo);
			}
		}

		private static void DeserializePost(JsonRepository<T, M> __instance)
		{
			if ( PostDeserialization != null && __instance is T repo )
			{
				PostDeserialization(repo);
			}
		}

		public static void ApplyPatch(Harmony harmony)
		{
			var orig = typeof(MonoSingleton<T>).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic);
			var post = typeof(RepositoryPatch<T, M>).GetMethod("Awake", BindingFlags.Static | BindingFlags.NonPublic);

			var origDeser = typeof(T).GetMethod("Deserialize", BindingFlags.Instance | BindingFlags.NonPublic);

			if ( origDeser == null || origDeser.IsVirtual )
			{
				Logger.Instance.info("No specific deserializer implementation for this repository. Falling back to JsonRepository (this is normal)");
				origDeser = typeof(JsonRepository<T, M>).GetMethod(
					"Deserialize", BindingFlags.Instance | BindingFlags.NonPublic);
			}
			
			var deserPost = typeof(RepositoryPatch<T, M>).GetMethod("DeserializePost", BindingFlags.Static | BindingFlags.NonPublic);

			harmony.Patch(orig, postfix: new HarmonyMethod(post));
			harmony.Patch(origDeser, postfix: new HarmonyMethod(deserPost));
			
			Logger.Instance.info("The repository patching <"+typeof(T).Name + ", " + typeof(M).Name + "> was done.");

		}

	}
}