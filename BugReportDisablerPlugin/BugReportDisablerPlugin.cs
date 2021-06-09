using System;
using GoingMedievalModLauncher;
using HarmonyLib;
using NSMedieval.Tools;
using NSMedieval.Tools.Debug;
using UnityEngine;
using Logger = GoingMedievalModLauncher.Logger;

namespace BugReportDisabler
{
    
    public class BugReportDisablerPlugin : IPlugin
    {
        
        public string Name => "Bug-Report Disabler";

        public string Description => "Manages the state of the Bug-Report Manager";
        public string Version => "v0.0.2";
        public bool activeState { get; set; }

        public void initialize()
        {
            activeState = true;
        }

        public void start(MonoBehaviour root)
        {
            if(!activeState) return;
            
            try
            {
                // disable the bug reporter
                Logger.getInstance().info("Disabling error reports ...");
                
                var bugReporterManager = UnityEngine.GameObject.FindObjectOfType<BugReporterManager>();
                Traverse.Create(bugReporterManager).Field("exceptionCaught").SetValue(true);
                
                // TODO: GameSettings.sendAutoReports should also be interesting for this
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

        public void disable(MonoBehaviour root)
        {
                        
            try
            {
                // disable the bug reporter
                Logger.getInstance().info("Disabling error reports ...");
                
                var bugReporterManager = UnityEngine.GameObject.FindObjectOfType<BugReporterManager>();
                Traverse.Create(bugReporterManager).Field("exceptionCaught").SetValue(false);
                
                // TODO: GameSettings.sendAutoReports should also be interesting for this
            }
            catch (Exception e)
            {
                Logger.getInstance().info(e.ToString());
                throw;
            }
        }
    }
    
}