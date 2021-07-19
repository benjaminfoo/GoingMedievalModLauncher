using System.Reflection;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.Controllers;
using NSMedieval.Production;

namespace GoingMedievalModLauncher.Engine
{
	public static class RoomTypePatch
	{

		private static FieldInfo _nameLoc = typeof(RoomType).GetField("nameLocalized", BindingFlags.NonPublic | BindingFlags.Instance);

		private static bool NameLocalized(ref string __result, ref RoomType __instance)
		{
			if ( __instance.GetID().Contains(":") )
			{
				var modS = __instance.GetID().Split(':');
				if (_nameLoc.GetValue(__instance).Equals(string.Empty))
					_nameLoc.SetValue(__instance, Singleton<LocalizationController>.Instance.GetText( modS[0] +":room_type_" + 
					modS[modS.Length-1]));
				__result = _nameLoc.GetValue(__instance) as string;
				return false;
			}
			return true;
		}

		public static void ApplyPatch(Harmony harmony)
		{
			var origLoc = typeof(RoomType).GetProperty("NameLocalized").GetMethod;
			var modLoc = typeof(RoomTypePatch).GetMethod("NameLocalized", BindingFlags.Static | BindingFlags.NonPublic);
			harmony.Patch(origLoc, new HarmonyMethod(modLoc));
			Launcher.LOGGER.Info("Room type patches were applied.");
		}

	}
}