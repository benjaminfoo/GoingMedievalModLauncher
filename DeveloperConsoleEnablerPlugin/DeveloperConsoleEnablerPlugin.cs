using GoingMedievalModLauncher;
using GoingMedievalModLauncher.plugins;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.DevConsole;
using NSMedieval.UI;
using UnityEngine;

namespace DeveloperConsoleEnablerPlugin
{
    public class DeveloperConsoleEnablerPlugin : IPlugin
    {

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

        public void disable(MonoBehaviour root)
        {
            var dtools = Traverse.Create(MonoSingleton<DeveloperToolsView>.Instance);
            dtools.Field("mainContainer").Method("SetActive", false).GetValue(); 
        }
    }
}