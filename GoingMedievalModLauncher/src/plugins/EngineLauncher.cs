using System;
using UnityEngine;

namespace GoingMedievalModLauncher
{
    public class EngineLauncher : MonoBehaviour
    {
        public void Start()
        {
            DontDestroyOnLoad(this);

            // load the integrated plugins
            PluginComponent reportDisabler = this.gameObject.AddComponent<PluginComponent>();
            reportDisabler.setup(new BugReportDisabler());    
              
            PluginComponent consoleEnabler = this.gameObject.AddComponent<PluginComponent>();
            consoleEnabler.setup(new DevConsole());
            
            PluginComponent cameraSettingsPP = this.gameObject.AddComponent<PluginComponent>();
            cameraSettingsPP.setup(new CameraSettings());
            
        }
    }
}