using System;
using GoingMedievalModLauncher;
using GoingMedievalModLauncher.plugins;
using HarmonyLib;
using NLog;
using NSEipix.Base;
using NSMedieval;
using UnityEngine;

namespace CameraSettingsPlusPlus
{
    public class CameraSettingsPlusPlus : IPlugin
    {

        internal static NLog.Logger LOGGER = LoggingManager.getLogger<CameraSettingsPlusPlus>();
        
        public float originalFarClip;
        public float originalHeightRangeMaxValue;

        public void initialize()
        {

        }

        public void start(MonoBehaviour root)
        {
        }

        public void update(MonoBehaviour root)
        {
            try
            {
                var cc = UnityEngine.GameObject.FindObjectOfType<RtsCamera>();
                if (cc != null)
                {
                    RenderSettings.fog = false;

                    originalFarClip = Camera.main.farClipPlane;
                    
                    // backup the original far clip plane value
                    Camera.main.farClipPlane = 99999;
                    QualitySettings.shadowDistance = 2048;

                    var traverse = Traverse.Create(cc.Settings);
    
                    // backup the original far clip plane value
                    originalHeightRangeMaxValue = 90; 
                    
                    traverse.Field("heightRange").Field("max").SetValue(999f);
                    traverse.Field("camSensitivity").SetValue(100f);
                    
                }
            }
            catch (Exception e)
            {
                LOGGER.Info(e.ToString());
                throw;
            }
            
        }

        public void disable(MonoBehaviour root)
        {
            try
            {
                var cc = UnityEngine.GameObject.FindObjectOfType<RtsCamera>();
                if (cc != null)
                {
                    RenderSettings.fog = true;
                    Camera.main.farClipPlane = originalFarClip;

                    var traverse = Traverse.Create(cc.Settings);

                    traverse.Field("heightRange").Field("max").SetValue(originalHeightRangeMaxValue);
                }
            }
            catch (Exception e)
            {
                LOGGER.Info(e.ToString());
                throw;
            }

        }
    }
}