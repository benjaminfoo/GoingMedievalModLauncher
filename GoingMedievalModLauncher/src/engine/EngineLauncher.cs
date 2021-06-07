using System;
using UnityEngine;

namespace GoingMedievalModLauncher
{
    public class EngineLauncher : MonoBehaviour
    {
        public void Start()
        {
            
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
            
        }
    }
}