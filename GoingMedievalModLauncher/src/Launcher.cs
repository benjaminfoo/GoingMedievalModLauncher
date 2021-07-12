using System;
using System.Reflection;
using System.Reflection.Emit;
using GoingMedievalModLauncher.Engine;
using GoingMedievalModLauncher.plugins;
using GoingMedievalModLauncher.ui;
using HarmonyLib;
using NSEipix.Base;
using NSEipix.Repository;
using NSMedieval.Crops;
using NSMedieval.Model;
using NSMedieval.Production;
using NSMedieval.Repository;
using NSMedieval.Research;
using UnityEngine;
using UnityEngine.SceneManagement;
using Logger = NLog.Logger;
using Object = UnityEngine.Object;

namespace GoingMedievalModLauncher
{
    /**
     * The Launcher class of the mod-loader
     * This is the entry-point of code execution for GM
     */
    public class Launcher
    {
        // is the startup of the launcher already done?
        private static bool _startupFinished;

        internal static readonly Logger LOGGER = LoggingManager.getLogger<Launcher>();

        // the entry point of the code execution
        public static void Main()
        {
            // we're hooking into the scene-manager to get a valid callback for executing our code 
            SceneManager.sceneLoaded += Startup;
            SceneManager.sceneLoaded += delegate(Scene arg0, LoadSceneMode mode)
            {
                if ( arg0.name == "MainScene" )
                {
                    ReplaceRepositories();
                }
            };
        }

        //Be careful, reflection and lowlevel stuff
        private static void ReplaceComponent<T, M>(FieldInfo[] fieldToCopy = null)
            where T : JsonRepository<T, M>
            where M : Model
        {
            TypeBuilder b = DynamicRepositoryBuilder<T, M>.GetTypeBuilder(
                $"Patched_{typeof(T).Name}");
            DynamicRepositoryBuilder<T, M>.OverrideDeserialize(b);
            Type t = DynamicRepositoryBuilder<T, M>.CompileResultType(b);
            var comp = Object.FindObjectOfType<T>();
            object[] variables = new object[0];
            if ( fieldToCopy != null )
            {
                variables = new object[fieldToCopy.Length];
                for (int i = 0; i < fieldToCopy.Length; i++)
                {
                    variables[i] = fieldToCopy[i].GetValue(comp);
                }
            }

            GameObject go = comp.gameObject;
            Object.DestroyImmediate(comp);
            var newComp = go.AddComponent(t);

            if(fieldToCopy != null)
                for (int i = 0; i < fieldToCopy.Length; i++)
                {
                    fieldToCopy[i].SetValue(newComp, variables[i]);
                }

        }

        //Be careful, reflection and lowlevel stuff
        private static void ReplaceRepositories()
        {

            ReplaceComponent<RoomTypeRepository, RoomType>(
                new[]
                {
                    typeof(RoomTypeRepository).GetField
                        ("roomDetectionMaterial", BindingFlags.Instance | BindingFlags.NonPublic)
                });
            LOGGER.Info("The room repository was replaced with a patched one.");
            ReplaceComponent<ResourceRepository, Resource>();
            LOGGER.Info("The resource repository was replaced with a patched one.");
            ReplaceComponent<ResearchRepository, ResearchModel>();
            LOGGER.Info("The research repository was replaced with a patched one.");
            ReplaceComponent<CropfieldRepository, Cropfield>();
            LOGGER.Info("The crop field repository was replaced with a patched one.");
            ReplaceComponent<ProductionRepository, Production>();
            LOGGER.Info("The resource repository was replaced with a patched one.");
        }

        /*
         * The startup of the launcher
         * This creates a native unity gameObject 
         */
        public static void Startup(Scene arg0, LoadSceneMode arg1)
        {
            LOGGER.Info("Initializing mod-loader!");

            if (_startupFinished) return;

            //Just be double safe.
            SceneManager.sceneLoaded -= Startup;

            try
            {
                var harmony = new Harmony("com.modloader.nsmeadival");
                    
                DebbugingPatches.ApplyPatches(harmony);
                MainMenuPatch.ApplyPatch(harmony);
                RoomTypePatch.ApplyPatch(harmony);
                LocalizationControllerPatch.ApplyPatch(harmony);
                PrefabRepositoryPatch.ApplyPatch(harmony);
            }
            catch (Exception e)
            {
                LOGGER.Info("Error happened while loading patches.\n" + e);
                throw;
            }
            
            try
            {

                LOGGER.Info("Mod-loader thread is running ...");

                // create a gameObject which we can use as a root reference to the scene-graph
                var modLoaderObject = new GameObject {name = "ModLoader"};
                modLoaderObject.AddComponent<EngineLauncher>();
                    
                // print out a nice little confirmation message that the plugin has been loaded
                LOGGER.Info("... initialization thread has been finished!");
                
                Singleton<PluginManager>.Instance.loadAssemblies();
            }
            catch (Exception e)
            {
                LOGGER.Info("An error occured: \n");
                LOGGER.Info(e.ToString());
                throw;
            }

            _startupFinished = true;
        }
    }
}