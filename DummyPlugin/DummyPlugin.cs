using GoingMedievalModLauncher;
using UnityEngine;

namespace DummyPlugin
{
    public class DummyPlugin : IPlugin
    {
        
        public string Name => "Dummy-Plugin";
        public string Description => "This plugin is for testing purposes and doesn't do anything except having a ridiculous long description.";
        public string ID => "dummy";
        public string Version => "v0.0.1";
        public bool activeState { get; set; }
        public void initialize()
        {
        }

        public void start(MonoBehaviour root)
        {
        }

        public void update(MonoBehaviour root)
        {
        }

        public void disable(MonoBehaviour root)
        {
        }
        
    }
}