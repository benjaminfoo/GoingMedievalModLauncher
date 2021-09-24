using System.Collections.Generic;
using System.IO;
using System.Linq;
using NSEipix.Base;

namespace GoingMedievalModLauncher.plugins
{
    public class PluginManager : Singleton<PluginManager>
    {

        private ICollection<PluginContainer> plugins;
        private PluginTree tree = new PluginTree();

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
        public ICollection<PluginContainer> LoadAssemblies()
        {
            // The directory where all assemblies / plugins / mods have to be stored
            DirectoryInfo dir = new DirectoryInfo(@"./mods");
            Launcher.LOGGER.Info("Looking for plugins in " + dir.FullName + " ...");
            
            foreach ( var pluginDir in dir.EnumerateDirectories() )
            {
                var p = PluginContainer.Create(pluginDir);
                if ( !(p is InvalidPluginContainer) )
                {
                    plugins.Add(p as PluginContainer);
                }
            }

            ValidateRequirements();
            
            ValidateDependencies();

            foreach ( PluginTree.TreeNode node in tree )
            {
                node.value.Init();
            }

            return plugins;

        }

        private void ValidateRequirements()
        {
            var pc = new List<PluginContainer>(plugins);

            IEnumerable<PluginTree.TreeNode> leaves;
            do
            {
                leaves = tree.leaves.ToList();
                foreach ( var leaf in leaves.ToArray() )
                {
                    var p = pc.ToList();
                    foreach ( var container in p )
                    {
                        if ( leaf.value.ID != container.Requirement ) continue;

                        tree.Add(new PluginTree.TreeNode(leaf, container));
                        pc.Remove(container);
                    }
                }
            } while ( !leaves.Equals(tree.leaves));

            foreach ( PluginContainer container in pc )
            {
                foreach ( var node in tree )
                {
                    if ( node.value == container )
                    {
                        Launcher.LOGGER.Info($"plugin {container.ID} was removed for Invalid requirement");
                        tree.Remove(node);
                        container.State = ContainerState.INVALID_REQUIREMENT;
                    }
                }
                
            }
        }

        private void ValidateDependencies()
        {
            while ( true )
            {
                var shouldRevalidate = false;
                var enu = tree.ToList();
                foreach ( PluginTree.TreeNode node in enu )
                {
                    if ( node.value.Dependencies == null || node.value.Dependencies.Length == 0 )
                    {
                        continue;
                    }

                    var deps = new List<string>(node.value.Dependencies);
                    foreach ( string dep in deps )
                    {
                        foreach ( PluginTree.TreeNode container in tree )
                        {
                            if ( dep == container.value.ID )
                            {
                                deps.Remove(dep);

                                break;
                            }
                        }
                    }

                    if ( deps.Count > 0 )
                    {
                        tree.Remove(node);
                        Launcher.LOGGER.Info($"plugin {node.value.ID} was removed for Invalid dependency");
                        node.value.State = ContainerState.INVALID_DEPENDENCY;
                        shouldRevalidate = true;
                    }

                }

                if ( shouldRevalidate )
                {
                    continue;
                }

                break;
            }
        }

    }
}