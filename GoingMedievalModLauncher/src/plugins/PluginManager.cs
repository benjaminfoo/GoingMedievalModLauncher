using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GoingMedievalModLauncher
{
    public class PluginManager
    {
        private static PluginManager instance;

        private ICollection<IPlugin> plugins;

        private PluginManager()
        {
            plugins = new List<IPlugin>();
        }

        public ICollection<IPlugin> GetPlugins()
        {
            return plugins;
        }

        public static PluginManager getInstance()
        {
            if (instance == null)
            {
                instance = new PluginManager();
            }

            return instance;
        }

        /**
         * Loads all assemblies by a given path
         */
        public ICollection<IPlugin> loadAssemblies()
        {
            // The directory where all assemblies / plugins / mods have to be stored
            DirectoryInfo dir = new DirectoryInfo(@"./mods");
            Logger.getInstance().info("Looking for plugins in " + dir.FullName + " ...");

            // load all the assemblies from the directory
            ICollection<Assembly> assemblies = new List<Assembly>();
            foreach (FileInfo dllFile in dir.GetFiles("*.dll"))
            {
                Logger.getInstance().info("Found dll: " + dllFile + " ...");
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile.FullName);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            }

            // Check if the assembly is valid (we don't want to load any dll within the mods-directory
            Type pluginType = typeof(IPlugin);
            ICollection<Type> validPlugins = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                if (assembly != null)
                {
                    Type[] types = assembly.GetTypes();
                    foreach (Type type in types)
                    {
                        if (type.IsInterface || type.IsAbstract)
                        {
                            continue;
                        }
                        else
                        {
                            if 
                            (
                                type.GetInterface(pluginType.FullName) != null &&
                                type.GetInterfaces().Contains(typeof(IPlugin))
                            )
                            {
                                validPlugins.Add(type);
                            }
                        }
                    }
                }
            }

            // initialize the plugins and start them from the unity-context
            foreach (Type type in validPlugins)
            {
                IPlugin plugin = (IPlugin) Activator.CreateInstance(type);
                plugins.Add(plugin);

                Logger.getInstance().info("Initializing plugin: " + plugin.Name + " - " + plugin.Version);
                plugin.initialize();
                
                // TODO:  plugin.start(doorstepGameObject);
            }

            return plugins;

        }
    }
}