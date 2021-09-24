using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Assets.Scripts.Models;
using GoingMedievalModLauncher.Engine;
using GoingMedievalModLauncher.plugins;
using GoingMedievalModLauncher.ui;
using GoingMedievalModLauncher.util;
using HarmonyLib;
using NSEipix.Base;
using NSEipix.Repository;
using NSMedieval;
using NSMedieval.Almanac;
using NSMedieval.Crops;
using NSMedieval.GameEventSystem;
using NSMedieval.Model;
using NSMedieval.Model.MapNew;
using NSMedieval.Production;
using NSMedieval.Repository;
using NSMedieval.Research;
using NSMedieval.StatsSystem;
using NSMedieval.Weather;
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
    public static class Launcher
    {
        // is the startup of the launcher already done?
        private static bool _startupFinished;

        internal static readonly Logger LOGGER = LoggingManager.GetLogger(typeof(Launcher));

        private static readonly Dictionary<Type, object> Repositories = new Dictionary<Type, object>();

        public static JsonRepository<T, M> getRepository<T, M>() where T : JsonRepository<T, M>
            where M : Model
        {
            object it = Repositories[typeof(T)];
            if ( it is JsonRepository<T, M> repo )
            {
                return repo;
            }

            return null;
        }

        // the entry point of the code execution
        public static void Main()
        {
            // we're hooking into the scene-manager to get a valid callback for executing our code 
            SceneManager.sceneLoaded += Startup;
        }

        //Be careful, reflection and lowlevel stuff
        private static void ReplaceComponent<T, M>(FieldInfo[] fieldToCopy = null)
            where T : JsonRepository<T, M>
            where M : Model
        {
            var comp = Object.FindObjectOfType<T>();
            if ( comp == null )
            {
                LOGGER.Info($"Unable to find component: {typeof(T).Name}");
                return;
            }
            TypeBuilder b = DynamicRepositoryBuilder<T, M>.GetTypeBuilder(
                $"Patched_{typeof(T).Name}");
            DynamicRepositoryBuilder<T, M>.OverrideDeserialize(b);
            Type t = DynamicRepositoryBuilder<T, M>.CompileResultType(b);
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
            
            Repositories.Add(typeof(T), newComp);

        }

        //Be careful, reflection and lowlevel stuff
        private static void ReplaceRepositories()
        {
            //Anything unrelated to Resources
            ReplaceComponent<RoomTypeRepository, RoomType>(
                new[]
                {
                    typeof(RoomTypeRepository).GetField
                        ("roomDetectionMaterial", BindingFlags.Instance | BindingFlags.NonPublic)
                });
            LOGGER.Info("The room repository was replaced with a patched one.");
            //TODO: Free from the value
            ReplaceComponent<ReligionRepository, ReligionConfig>();
            LOGGER.Info("The religion repository was replaced with a patched one.");
            
            //Repositories that ad new resources (prefab, for example)
            ReplaceComponent<ResourceRepository, Resource>();
            LOGGER.Info("The resource repository was replaced with a patched one.");
            //TODO: Adding custom event classes
            ReplaceComponent<GameEventSettingsRepository, GameEvent>();
            LOGGER.Info("The game event repository was replaced with a patched one.");
            ReplaceComponent<EffectorRepository, StatEffector>();
            LOGGER.Info("The effector repository was replaced with a patched one.");
            ReplaceComponent<WoundsRepository, StatEffectorWound>();
            LOGGER.Info("The wound effector repository was replaced with a patched one.");
            ReplaceComponent<AlmanacRepository, Almanac>();
            LOGGER.Info("The almanac effector repository was replaced with a patched one.");

            //Repositories that require other ids/repositories
            ReplaceComponent<EventGroupRepository, EventGroup>();
            LOGGER.Info("The event group repository was replaced with a patched one.");
            ReplaceComponent<ResearchRepository, ResearchModel>();
            LOGGER.Info("The research repository was replaced with a patched one.");
            ReplaceComponent<PlantPrefabRepository, PlantPrefabs>();
            LOGGER.Info("The plant prefab repository was replaced with a patched one.");
            ReplaceComponent<CropfieldRepository, Cropfield>();
            LOGGER.Info("The crop field repository was replaced with a patched one.");
            ReplaceComponent<ProductionRepository, Production>();
            LOGGER.Info("The resource repository was replaced with a patched one.");
            ReplaceComponent<WeatherEventRepository, WeatherEvent>();
            LOGGER.Info("The weather repository was replaced with a patched one.");
            ReplaceComponent<AlmanacEntriesRepository, AlmanacEntries>();
            LOGGER.Info("The almanac entry effector repository was replaced with a patched one.");
        }

        /*
         * The startup of the launcher
         * This creates a native unity gameObject 
         */
        private static void Startup(Scene arg0, LoadSceneMode arg1)
        {
            LOGGER.Info("Initializing mod-loader!");

            if (_startupFinished) return;

            //Just be double safe.
            SceneManager.sceneLoaded -= Startup;

            try
            {
                var harmony = new Harmony("com.modloader.nsmeadival");
                    
                //DebbugingPatches.ApplyPatches(harmony);
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

                LOGGER.Info("Mod-loader is running ...");
                
                Singleton<PluginManager>.Instance.LoadAssemblies();

                // create a gameObject which we can use as a root reference to the scene-graph
                var modLoaderObject = new GameObject {name = "ModLoader"};
                modLoaderObject.AddComponent<EngineLauncher>();
                    
                // print out a nice little confirmation message that the plugin has been loaded
                LOGGER.Info("... initialization thread has been finished!");
            }
            catch (Exception e)
            {
                LOGGER.Info("An error occured: \n");
                LOGGER.Info(e.ToString());
                throw;
            }
            //For replacements that are required before the game map
            ReplaceComponent<MapRepository, Map>();
            LOGGER.Info("The Maptype repository was replaced with a patched one.");
            ReplaceComponent<MapSizeRepository, MapSize>();
            LOGGER.Info("The Mapsize repository was replaced with a patched one.");
            ReplaceComponent<VillageNameRepository, VillageNames>();
            LOGGER.Info("The village name repository was replaced with a patched one.");
            //TODO: Free me from the enums!
            //TODO: Please give me a UI!
            ReplaceComponent<GameDifficultyRepository, GameDifficulty>();
            LOGGER.Info("The game difficulty repository was replaced with a patched one.");
            
            SceneManager.sceneLoaded += delegate(Scene aarg0, LoadSceneMode mode)
            {
                if ( aarg0.name == "MainScene" )
                {
                    ReplaceRepositories();
                }
            };

            _startupFinished = true;
        }
    }
}