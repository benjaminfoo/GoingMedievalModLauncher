using System;
using GoingMedievalModLauncher;
using HarmonyLib;
using NSEipix.Base;
using NSMedieval;
using UnityEngine;
using Logger = GoingMedievalModLauncher.Logger;

namespace CameraSettingsPlusPlus
{
    public class CameraSettingsPlusPlus : IPlugin
    {
        public string Name => "Camera-Settings";
        
        public string Description => "Increases rendering-, zoom- & shadow-distance - may slow down performance.";
        public string ID => "cameraPlusPlus";
        public string Version => "v0.0.2";
        
        public bool activeState { get; set; }

        public float originalFarClip;
        public float originalHeightRangeMaxValue;

        public void initialize()
        {
            activeState = true;

        }

        public void start(MonoBehaviour root)
        {
        }

        public void update(MonoBehaviour root)
        {
            if(!activeState) return;

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
                Logger.Instance.info(e.ToString());
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
                Logger.Instance.info(e.ToString());
                throw;
            }

            activeState = false;
            
        }
    }
}