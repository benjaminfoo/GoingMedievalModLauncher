using System;
using GoingMedievalModLauncher;
using HarmonyLib;
using NSMedieval;
using UnityEngine;
using Logger = GoingMedievalModLauncher.Logger;

namespace CameraSettingsPlusPlus
{
    public class CameraSettingsPlusPlus : IPlugin
    {
        public string Name => "Camera-Settings";
        public string Version => "v0.0.1";

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
                    Camera.main.farClipPlane = 99999;

                    var traverse = Traverse.Create(cc.Settings);

                    traverse.Field("heightRange").Field("max").SetValue(999999f);
                }
            }
            catch (Exception e)
            {
                Logger.getInstance().info(e.ToString());
                throw;
            }
        }
    }
}