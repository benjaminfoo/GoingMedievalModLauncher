﻿using System;
using System.Reflection;
using System.Threading;
using GoingMedievalModLauncher.ui;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval.Tools.Debug;
using NSMedieval.UI;
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

            new Thread(() =>
            {
                try
                {
                    
                    var harmony = new Harmony("com.modloader.nsmeadival");
                    
                    MainMenuPatch.ApplyPatch(harmony);

                    Singleton<PluginManager>.Instance.loadAssemblies();
                    
                    // wait a short amount of time in order to let the game initialize itself
                    Thread.Sleep(2500);
                   

                    Logger.Instance.info("Mod-loader thread is running ...");

                    // create a gameObject which we can use as a root reference to the scene-graph
                    var modLoaderObject = new GameObject {name = "ModLoader"};
                    modLoaderObject.AddComponent<EngineLauncher>();
                    
                    // print out a nice little confirmation message that the plugin has been loaded
                    Logger.Instance.info("... initialization thread has been finished!");
                }
                catch (Exception e)
                {
                    Logger.Instance.info("An error occured: \n");
                    Logger.Instance.info(e.ToString());
                    throw;
                }
            }).Start();

            startupFinished = true;
        }
    }
}