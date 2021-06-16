using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NSEipix.Base;
using UnityEngine;

namespace GoingMedievalModLauncher
{
    public class PluginManager : Singleton<PluginManager>
    {

        private ICollection<PluginContainer> plugins;

        private PluginManager()
        {
            plugins = new List<PluginContainer>();
        }

        public ICollection<PluginContainer> GetPlugins()
        {
            return plugins;
        }

        /**
         * Loads all assemblies by a given path
         */
        public ICollection<PluginContainer> loadAssemblies()
        {
            // The directory where all assemblies / plugins / mods have to be stored
            DirectoryInfo dir = new DirectoryInfo(@"./mods");
            Logger.Instance.info("Looking for plugins in " + dir.FullName + " ...");
            
            foreach ( var pluginDir in dir.EnumerateDirectories() )
            {
                var p = PluginContainer.Create(pluginDir);
                if ( !(p is InvalidPluginContainer) )
                {
                    plugins.Add(p);
                }
            }

            return plugins;

        }
    }
}