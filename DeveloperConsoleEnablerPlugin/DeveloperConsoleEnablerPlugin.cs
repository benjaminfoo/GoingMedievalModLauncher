using GoingMedievalModLauncher;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.DevConsole;
using UnityEngine;

namespace DeveloperConsoleEnablerPlugin
{
    public class DeveloperConsoleEnablerPlugin : IPlugin
    {

        public string Name => "Developer-Console enabler";
        public string Version => "v0.0.1";

        public void initialize()
        {

        }

        public void start(MonoBehaviour root)
        {

        }

        public void update(MonoBehaviour root)
        {
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
        
    }
}