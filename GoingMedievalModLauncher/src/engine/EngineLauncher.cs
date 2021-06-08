using System;
using GoingMedievalModLauncher.ui;
using HarmonyLib;
using NSMedieval.Tools;
using NSMedieval.Tools.BugReporting;
using UnityEngine;

namespace GoingMedievalModLauncher
{
    public class EngineLauncher : MonoBehaviour
    {
        public void Start()
        {
            
            // update the game version to indicate this is a modded version
            var gameVersion = UnityEngine.GameObject.FindObjectOfType<GameVersion>();
            var t = Traverse.Create(gameVersion);
            t.Field("suffix").SetValue(" - mods active");
            t.Method("Start").GetValue();
            
            // dont destroy the engines object when loading another scene, etc.
            DontDestroyOnLoad(this);

            // Load all the mods / plugins / assemblies from the mods directory
            var loadedPlugins = PluginManager.getInstance().loadAssemblies();

            // Initialize a new gameObject for each loaded plugin: 
            // - set it up
            // - attach the plugin to the gameObject
            // - make use of unity's lifecycle (start, update, etc.)
            foreach (var loadedPlugin in loadedPlugins)
            {
                PluginComponent pluginComponent = this.gameObject.AddComponent<PluginComponent>();
                pluginComponent.setup(loadedPlugin);
            }

            Logger.getInstance().info("Showing mod-manager window...");
            var modManagerWindow = gameObject.AddComponent<ModManagerWindow>();
            
        }
        
    }
    
}