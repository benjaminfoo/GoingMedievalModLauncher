using UnityEngine;

namespace GoingMedievalModLauncher.plugins
{
    // The PluginComponent is a simple monobehavior which contains a reference to an IPlugin-Instance - 
    // this way, a plugin / mod can make use of the native lifecycle of an object within the unity-engine.
    //
    // See the IPluginContainer-Class for more detailed information about each method.
    //     
    public class PluginComponent : MonoBehaviour
    {
        
        public IPluginContainer pluginImpl;

        public void setup(IPluginContainer plugin)
        {
            pluginImpl = plugin;
        }

        void Start()
        {
            if(pluginImpl == null || !pluginImpl.ActiveState || pluginImpl.plugin == null) return;
            pluginImpl.plugin.start(this);
        }

        void Update()
        {
            if(pluginImpl == null || !pluginImpl.ActiveState|| pluginImpl.plugin == null) return;
            pluginImpl.plugin.update(this);
        }

    }
    
}