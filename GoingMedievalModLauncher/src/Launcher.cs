using System;
using System.Threading;
using HarmonyLib;
using NSMedieval.Tools.Debug;
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
            Logger.getInstance().info("Initializing mod-loader!");

            if (startupFinished) return;

            new Thread(() =>
            {
                try
                {
                    // wait a short amount of time in order to let the game initialize itself
                    Thread.Sleep(2500);

                    Logger.getInstance().info("Mod-loader thread is running ...");

                    // create a gameObject which we can use as a root reference to the scene-graph
                    var modLoaderObject = new GameObject {name = "ModLoader"};
                    modLoaderObject.AddComponent<EngineLauncher>();
                    
                    // print out a nice little confirmation message that the plugin has been loaded
                    Logger.getInstance().info("... initialization thread has been finished!");
                }
                catch (Exception e)
                {
                    Logger.getInstance().info("An error occured: \n");
                    Logger.getInstance().info(e.ToString());
                    throw;
                }
            }).Start();

            startupFinished = true;
        }
    }
}