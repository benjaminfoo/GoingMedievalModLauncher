using System;
using GoingMedievalModLauncher;
using GoingMedievalModLauncher.plugins;
using HarmonyLib;
using NLog;
using NSMedieval.Tools;
using NSMedieval.Tools.Debug;
using UnityEngine;
using Logger = GoingMedievalModLauncher.Logger;

namespace BugReportDisabler
{
    
    public class BugReportDisablerPlugin : IPlugin
    {

        internal NLog.Logger LOGGER = LogManager.GetLogger(nameof(BugReportDisablerPlugin));

        public void initialize()
        {
        }

        public void start(MonoBehaviour root)
        {
            try
            {
                // disable the bug reporter
                LOGGER.Info("Disabling error reports ...");
                
                var bugReporterManager = UnityEngine.GameObject.FindObjectOfType<BugReporterManager>();
                Traverse.Create(bugReporterManager).Field("exceptionCaught").SetValue(true);
                
                // TODO: GameSettings.sendAutoReports should also be interesting for this
            }
            catch (Exception e)
            {
                LOGGER.Info(e.ToString());
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
                LOGGER.Info("Disabling error reports ...");
                
                var bugReporterManager = UnityEngine.GameObject.FindObjectOfType<BugReporterManager>();
                Traverse.Create(bugReporterManager).Field("exceptionCaught").SetValue(false);
                
                // TODO: GameSettings.sendAutoReports should also be interesting for this
            }
            catch (Exception e)
            {
                LOGGER.Info(e.ToString());
                throw;
            }
        }
    }
    
}