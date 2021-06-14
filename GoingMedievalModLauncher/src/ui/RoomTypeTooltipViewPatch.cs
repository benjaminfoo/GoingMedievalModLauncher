using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.Controllers;
using NSMedieval.Model;
using NSMedieval.Production;
using NSMedieval.UI;

namespace GoingMedievalModLauncher.ui
{
	public static class RoomTypeTooltipViewPatch
	{

		private static FieldInfo roomtype = typeof(RoomTypeTooltipView).GetField("roomType", BindingFlags.NonPublic |
		 BindingFlags.Instance);
		private static FieldInfo MustHaveColor = typeof(RoomTypeTooltipView).GetField("MustHaveColor", BindingFlags
		.NonPublic | BindingFlags.Static);
		private static FieldInfo CannotHaveColor = typeof(RoomTypeTooltipView).GetField("CannotHaveColor", BindingFlags
			.NonPublic | BindingFlags.Static);

		private static MethodInfo ColorText = typeof(RoomTypeTooltipView).GetMethod(
			"ColorText", BindingFlags.NonPublic | BindingFlags.Instance);

		private static MethodInfo GetLocalizedBuildingsList =
			typeof(RoomTypeTooltipView).GetMethod("GetLocalizedBuildingsList", BindingFlags.NonPublic | BindingFlags.Instance);

		private static bool GetTextLines(RoomTypeTooltipView __instance, ref List<KeyTooltipStylePair> __result)
		{
			
			var roomType = (RoomType) roomtype.GetValue(__instance);

			if ( roomType != null && roomType.GetID().Contains(":") )
			{

				var modS = roomType.GetID().Split(':');
				var cannotHaveColor = CannotHaveColor.GetValue(__instance);
				var mustHaveColor = MustHaveColor.GetValue(__instance);

				List<KeyTooltipStylePair> tooltipStylePairList = new List<KeyTooltipStylePair>();
			    string id1 = (string)ColorText.Invoke(__instance ,new object[]{ Singleton<LocalizationController>
			    .Instance
			    .GetText
			    ("room_must_have") + 
			    ":", mustHaveColor});
			    string id2 =  (string)ColorText.Invoke(__instance ,new object[]{ Singleton<LocalizationController>.Instance
			    .GetText
			    ("room_cannot_have") + ":", cannotHaveColor});
			    string orSeparator = " " + Singleton<LocalizationController>.Instance.GetText("list_or") + " ";
			    StringBuilder sbMustHave = new StringBuilder();
			    roomType.MustHave.ForEach((Action<RoomTypeMustHave>) (item =>
			    {
			      string str;
			      if ( item.TextKey == null || item.TextKey.Equals(string.Empty) )
			      {
				      str = GetLocalizedBuildingsList.Invoke(__instance, new object[] {item.Buildings, orSeparator,
			      ", "}) as string;
				      
			        if (item.Buildings.Count >= 2)
			          str = str ?? "";
			        
			      }
			      else
			        str = Singleton<LocalizationController>.Instance.GetText(item.TextKey);
			      if (item != roomType.MustHave.First<RoomTypeMustHave>())
			        sbMustHave.Append("\n");
			      if (item.MaxCount == item.MinCount)
			        sbMustHave.Append(string.Format("{0} {1}x {2}", (object) Singleton<LocalizationController>.Instance.GetText("list_exactly"), (object) item.MinCount, (object) str));
			      else if (item.MaxCount <= 0)
			        sbMustHave.Append(string.Format("{0} {1}x {2}", (object) Singleton<LocalizationController>.Instance.GetText("list_at_least"), (object) item.MinCount, (object) str));
			      else
			        sbMustHave.Append(string.Format("{0} - {1}x {2}", (object) item.MinCount, (object) item.MaxCount, (object) str));
			      if (item == roomType.MustHave.Last<RoomTypeMustHave>())
			        return;
			      sbMustHave.Append(", ");
			    }));
			    StringBuilder stringBuilder = new StringBuilder();
			    if (roomType.TextKeyCantHaveBuildings == null || roomType.TextKeyCantHaveBuildings.Equals(string.Empty))
			    {
			      stringBuilder.Append(GetLocalizedBuildingsList.Invoke( __instance, new object[]{roomType.CantHave, ",\n", 
			      ",\n"}) as string);
			      if (roomType.CantHaveOtherProductionBuildings)
			        stringBuilder.Append("\n" + Singleton<LocalizationController>.Instance.GetText("room_cant_have_other_prod_buildings"));
			    }
			    else
			      stringBuilder.Append(Singleton<LocalizationController>.Instance.GetText(roomType.TextKeyCantHaveBuildings));
			    tooltipStylePairList.Add(new KeyTooltipStylePair(Singleton<LocalizationController>.Instance.GetText("room_click_to_select"), MonoSingleton<TooltipStyles>.Instance.DefaultLineStyle));
			    tooltipStylePairList.Add(new KeyTooltipStylePair("\n", MonoSingleton<TooltipStyles>.Instance.SpacerStyle));
			    tooltipStylePairList.Add(new KeyTooltipStylePair(Singleton<LocalizationController>.Instance.GetText
			    (modS[0]+":room_info_" + modS[modS.Length - 1]), MonoSingleton<TooltipStyles>.Instance.DescriptionLineStyle));
			    if (sbMustHave.Length > 0)
			    {
				  tooltipStylePairList.Add(new KeyTooltipStylePair("\n", MonoSingleton<TooltipStyles>.Instance.SpacerStyle));
			      tooltipStylePairList.Add(new KeyTooltipStylePair(id1, MonoSingleton<TooltipStyles>.Instance.DefaultLineStyle));
			      tooltipStylePairList.Add(new KeyTooltipStylePair(sbMustHave.ToString(), MonoSingleton<TooltipStyles>.Instance.AttributeLineStyle));
			    }
			    if (stringBuilder.Length > 0)
			    { 
				  tooltipStylePairList.Add(new KeyTooltipStylePair("\n", MonoSingleton<TooltipStyles>.Instance.SpacerStyle));
			      tooltipStylePairList.Add(new KeyTooltipStylePair(id2, MonoSingleton<TooltipStyles>.Instance.DefaultLineStyle));
			      tooltipStylePairList.Add(new KeyTooltipStylePair(stringBuilder.ToString(), MonoSingleton<TooltipStyles>.Instance.AttributeLineStyle));
			    }
			    if (roomType.MinimumArea > 0)
			    {
				  tooltipStylePairList.Add(new KeyTooltipStylePair("\n", MonoSingleton<TooltipStyles>.Instance.SpacerStyle));
			      tooltipStylePairList.Add(new KeyTooltipStylePair(string.Format("{0} {1}", (object) Singleton<LocalizationController>.Instance.GetText("room_minimum_area"), (object) roomType.MinimumArea), MonoSingleton<TooltipStyles>.Instance.AttributeLineStyle));
			    }
			    tooltipStylePairList.Add(new KeyTooltipStylePair("\n", MonoSingleton<TooltipStyles>.Instance.SpacerStyle));
			    tooltipStylePairList.Add(new KeyTooltipStylePair(Singleton<LocalizationController>.Instance.GetText
			    (modS[0] + ":room_effect_" + modS[modS.Length -1]), MonoSingleton<TooltipStyles>.Instance.DefaultLineStyle));
			    __result = tooltipStylePairList;
			    return false;
			}
			return true;
		}

		public static void ApplyPatch(Harmony harmony)
		{
			var orig = typeof(RoomTypeTooltipView).GetMethod("GetTextLines", BindingFlags.NonPublic | BindingFlags.Instance);
			var pref = typeof(RoomTypeTooltipViewPatch).GetMethod("GetTextLines", BindingFlags.NonPublic | BindingFlags.Static);
			harmony.Patch(orig, new HarmonyMethod(pref));
			Logger.Instance.info("Room Type Tooltip View patches were applied.");
		}

	}
}