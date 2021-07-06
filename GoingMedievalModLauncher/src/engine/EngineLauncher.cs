using GoingMedievalModLauncher.plugins;
using GoingMedievalModLauncher.ui;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.Tools;
using UnityEngine;

namespace GoingMedievalModLauncher.Engine
{
    // The EngineLauncher is the bridge from outside to the inside of the unity-process (where gm resides).
    
    // The EngineLauncher gets loads any provided mod from the /mods directory and instantiates the lifecycle of each
    // plugin within the unity-engine.
    public class EngineLauncher : MonoBehaviour
    {
        public void Awake()
        {

            // update the game version to indicate this is a modded version
            var gameVersion = UnityEngine.GameObject.FindObjectOfType<GameVersion>();
            var t = Traverse.Create(gameVersion);
            t.Field("suffix").SetValue(" - mods active");
            t.Method("Start").GetValue();

            // dont destroy the engines object when loading another scene, etc.
            DontDestroyOnLoad(this);

            // Load all the mods / plugins / assemblies from the mods directory
            var loadedPlugins = Singleton<PluginManager>.Instance.GetPlugins();

            // Initialize a new gameObject for each loaded plugin: 
            // - set it up
            // - attach the plugin to the gameObject
            // - make use of unity's lifecycle (start, update, etc.)
            foreach (var loadedPlugin in loadedPlugins)
            {
                PluginComponent pluginComponent = this.gameObject.AddComponent<PluginComponent>();
                pluginComponent.setup(loadedPlugin.plugin);
            }

            // Show a fancy ui to display and control every loaded mod at runtime
            Logger.Instance.info("Showing mod-manager window...");
            var modManagerWindow = gameObject.AddComponent<ModManagerWindow>();

        }

    }
    
}