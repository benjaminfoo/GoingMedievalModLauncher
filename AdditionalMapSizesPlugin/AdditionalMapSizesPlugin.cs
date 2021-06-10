using GoingMedievalModLauncher;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.Model.MapNew;
using NSMedieval.Repository;
using UnityEngine;

namespace AdditionalMapSizesPlugin
{
    public class AdditionalMapSizesPlugin : IPlugin
    {
        public string Name => "Additional Map Sizes";
        public string Description => "Adds map sizes to the new-game map-selection screen.";
        public string Version => "v0.0.1";
        public bool activeState { get; set; }
        public void initialize()
        {
            activeState = true;
        }

        public void disable(MonoBehaviour root)
        {
            remove("128 x 128");
            remove("256 x 256");
            remove("512 x 512");
        }

        public void update(MonoBehaviour root)
        {
            
        }

        public void start(MonoBehaviour root)
        {

            add(create("128 x 128", 128, 16, 128, 3));
            add(create("256 x 256", 256, 16, 256, 3));
            add(create("512 x 512", 512, 16, 512, 3));
            
        }

        public MapSize create(string name, int width, int height, int length, int blockHeight = 3)
        {
            var newMap = new MapSize();
            var twoFiveSixTraverser = Traverse.Create(newMap);
            twoFiveSixTraverser.Field("id").SetValue(name);
            twoFiveSixTraverser.Field("width").SetValue(width);
            twoFiveSixTraverser.Field("height").SetValue(height);
            twoFiveSixTraverser.Field("length").SetValue(length);
            twoFiveSixTraverser.Field("blockHeight").SetValue(blockHeight);
            twoFiveSixTraverser.Field("shownInRelease").SetValue(true);
            return newMap;
        }

        public void add(MapSize mapSize)
        {
            MonoSingleton<MapSizeRepository>.Instance.Add(mapSize);
            MonoSingleton<MapSizeRepository>.Instance.Reload();
        }

        public void remove(string mapSizeId)
        {
            MonoSingleton<MapSizeRepository>.Instance.RemoveByID(mapSizeId);
            MonoSingleton<MapSizeRepository>.Instance.Reload();
        }

    }
}