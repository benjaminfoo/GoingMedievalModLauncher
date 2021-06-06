using System;
using HarmonyLib;
using NSMedieval.Tools;
using NSMedieval.Tools.BugReporting;
using NSMedieval.Tools.Debug;
using UnityEngine;

namespace GoingMedievalModLauncher
{
    public class BugReportDisabler : IPlugin
    {

        public string Name => "Bug-Report Disabler";
        public string Version => "v0.0.1";

        public void initialize()
        {

        }

        public void start(MonoBehaviour root)
        {
            try
            {
                // disable the bug reporter
                Logger.getInstance().info("Disabling error reports ...");
                
                var bugReporterManager = UnityEngine.GameObject.FindObjectOfType<BugReporterManager>();
                Traverse.Create(bugReporterManager).Field("exceptionCaught").SetValue(true);
                
                // update the game version to indicate this is a modded version
                var gameVersion = UnityEngine.GameObject.FindObjectOfType<GameVersion>();
                var t = Traverse.Create(gameVersion);
                t.Field("suffix").SetValue(" - mods active");
                t.Method("Start").GetValue();
            }
            catch (Exception e)
            {
                Logger.getInstance().info(e.ToString());
                throw;
            }
        }

        public void update(MonoBehaviour root)
        {

        }
        
    }
}