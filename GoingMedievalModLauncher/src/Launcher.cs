using System;
using System.Threading;
using GoingMedievalModLauncher.Engine;
using GoingMedievalModLauncher.ui;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.Crops;
using NSMedieval.Production;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoingMedievalModLauncher
{
    /**
     * The Launcher class of the mod-loader
     * This is the entry-point of code execution for GM
     */
    public class Launcher
    {
        // is the startup of the launcher already done?
        private static bool startupFinished = false;

        // the entry point of the code execution
        public static void Main()
        {
            // we're hooking into the scene-manager to get a valid callback for executing our code 
            SceneManager.sceneLoaded += startup;
        }

        /*
         * The startup of the launcher
         * This creates a native unity gameObject 
         */
        public static void startup(Scene arg0, LoadSceneMode arg1)
        {
            Logger.Instance.info("Initializing mod-loader!");

            if (startupFinished) return;

            //Just be double safe.
            SceneManager.sceneLoaded -= startup;

            try
            {
                var harmony = new Harmony("com.modloader.nsmeadival");
                    
                MainMenuPatch.ApplyPatch(harmony);
                RoomTypePatch.ApplyPatch(harmony);
                RoomTypeTooltipViewPatch.ApplyPatch(harmony);
                RepositoryPatch<RoomTypeRepository, RoomType>.ApplyPatch(harmony);
                RepositoryPatch<CropfieldRepository, Cropfield>.ApplyPatch(harmony);
            }
            catch (Exception e)
            {
                Logger.Instance.info("Error happened while loading patches.\n" + e);
                throw;
            }
            
            try
            {

                MainMenuPatch.OnMenuStartPost += delegate
                {
                    Logger.Instance.info("Mod-loader thread is running ...");

                    // create a gameObject which we can use as a root reference to the scene-graph
                    var modLoaderObject = new GameObject {name = "ModLoader"};
                    modLoaderObject.AddComponent<EngineLauncher>();
                    
                    // print out a nice little confirmation message that the plugin has been loaded
                    Logger.Instance.info("... initialization thread has been finished!");  
                };
                    
                Singleton<PluginManager>.Instance.loadAssemblies();
            }
            catch (Exception e)
            {
                Logger.Instance.info("An error occured: \n");
                Logger.Instance.info(e.ToString());
                throw;
            }

            startupFinished = true;
        }
    }
}