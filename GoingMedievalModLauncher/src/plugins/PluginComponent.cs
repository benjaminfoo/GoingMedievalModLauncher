using UnityEngine;

namespace GoingMedievalModLauncher.plugins
{
    // The PluginComponent is a simple monobehavior which contains a reference to an IPlugin-Instance - 
    // this way, a plugin / mod can make use of the native lifecycle of an object within the unity-engine.
    //
    // See the IPlugin-Class for more detailed information about each method.
    //     
    public class PluginComponent : MonoBehaviour
    {
        
        public IPlugin pluginImpl;

        public void setup(IPlugin plugin)
        {
            this.pluginImpl = plugin;
        }

        void Start()
        {
            if(pluginImpl == null) return;
            pluginImpl.start(this);
        }

        void Update()
        {
            if(pluginImpl == null) return;
            pluginImpl.update(this);
        }

    }
    
}