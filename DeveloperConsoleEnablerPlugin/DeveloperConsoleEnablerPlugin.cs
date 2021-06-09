using GoingMedievalModLauncher;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.DevConsole;
using NSMedieval.UI;
using UnityEngine;

namespace DeveloperConsoleEnablerPlugin
{
    public class DeveloperConsoleEnablerPlugin : IPlugin
    {

        public string Name => "Developer-Console enabler";
        public string Description => "Enables the developer console.";

        public string Version => "v0.0.2";
        
        public bool activeState { get; set; }

        public void initialize()
        {
            activeState = true;
        }

        public void start(MonoBehaviour root)
        {

        }

        public void update(MonoBehaviour root)
        {
            if(!activeState) return;

            if (Input.GetKeyDown(KeyCode.L))
            {
                MonoSingleton<DeveloperToolsView>.Instance.Open();
                    
                var dtools = Traverse.Create(MonoSingleton<DeveloperToolsView>.Instance);
                dtools.Field("mainContainer").Method("SetActive", true).GetValue();
            }

            if (Input.GetKeyDown(KeyCode.K))
            {
                MonoSingleton<DeveloperToolsView>.Instance.Open();
                    
                var dtools = Traverse.Create(MonoSingleton<DeveloperToolsView>.Instance);
                dtools.Field("mainContainer").Method("SetActive", false).GetValue();
            }
        }

        public void disable(MonoBehaviour root)
        {
            activeState = false; 
            var dtools = Traverse.Create(MonoSingleton<DeveloperToolsView>.Instance);
            dtools.Field("mainContainer").Method("SetActive", false).GetValue(); 
        }
    }
}