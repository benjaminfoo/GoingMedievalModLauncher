using System;
using UnityEngine;

namespace GoingMedievalModLauncher
{
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