using System;
using GoingMedievalModLauncher;
using GoingMedievalModLauncher.plugins;
using GoingMedievalModLauncher.util;
using HarmonyLib;
using NLog;
using NSMedieval;
using NSMedieval.Tools;
using NSMedieval.Tools.Debug;
using NSMedieval.UI;
using UnityEngine;

namespace BugReportDisabler
{
    
    public class BugReportDisablerPlugin : IPlugin
    {

        internal NLog.Logger LOGGER = LoggingManager.GetLogger<BugReportDisablerPlugin>();

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
                var settingsCont = UnityEngine.GameObject.FindObjectOfType<OptionsController>();
                Traverse.Create(bugReporterManager).Field("exceptionCaught").SetValue(true);
                
                var globSettings = Traverse.Create(settingsCont).Field("globalSettings").GetValue() as GlobalSettings;
                globSettings?.SetSendAutoReports(false);
                
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
                var settingsCont = UnityEngine.GameObject.FindObjectOfType<OptionsController>();
                Traverse.Create(bugReporterManager).Field("exceptionCaught").SetValue(false);
                
                var globSettings = Traverse.Create(settingsCont).Field("globalSettings").GetValue() as GlobalSettings;
                globSettings?.SetSendAutoReports(true);
                
            }
            catch (Exception e)
            {
                LOGGER.Info(e.ToString());
                throw;
            }
        }
    }
    
}