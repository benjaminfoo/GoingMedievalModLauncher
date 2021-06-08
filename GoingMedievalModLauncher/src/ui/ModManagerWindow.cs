using System;
using System.Text;
using HarmonyLib;
using NSEipix;
using NSEipix.Base;
using NSEipix.Repository;
using NSMedieval;
using NSMedieval.DevConsole;
using NSMedieval.GameEventSystem;
using NSMedieval.InfoMessages;
using NSMedieval.Model;
using NSMedieval.Repository;
using NSMedieval.Sound;
using NSMedieval.Tools;
using NSMedieval.Tools.Debug;
using NSMedieval.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.Examples;
using Object = UnityEngine.Object;

namespace GoingMedievalModLauncher.ui
{
    public class ModManagerWindow : UIWindow
    {
        public ModManagerWindow()
        {
            // BlackBarMessageController.Instance.ShowClickableBlackBarMessage("Hello World!", Vector3.one);
        }

        public void Start()
        {
            this.windowTitle = "Going Medieval - Mod Manager";
            this.windowId = 1;
            this.windowRect = new Rect(10, 10, 600, 300);
            this.shown = true;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                // toggle the visibility via key-press
                shown = !shown;
            }
        }


        void OnGUI()
        {
            if (shown)
            {
                // Register the window. We create two windows that use the same function
                // Notice that their IDs differ
                GUI.backgroundColor = Color.black;
                windowRect = GUI.Window(windowId, windowRect, renderWindow, windowTitle);
            }
        }


        public void renderWindow(int id)
        {
            // we HAVE to call this in order to call the renderWindow function from the abstract base class
            base.renderWindow(windowId);
            
            
            int index = 1;
            int offsetY = 30;
            foreach (var plugin in PluginManager.getInstance().GetPlugins())
            {
                var nameLabelRect = new Rect(10, 40 + (index * offsetY), 150, 20);
                var versionLabelRect = new Rect(350, 40 + (index * offsetY), 100, 20);
                var descriptionLabelRect = new Rect(150, 40 + (index * offsetY), 190, 20);

                
                GUI.Label(nameLabelRect, plugin.Name);
                GUI.Label(descriptionLabelRect, plugin.Description);
                GUI.Label(versionLabelRect, plugin.Version);

                var enableButtonRect = new Rect(450, 40 + (index * offsetY), 100, 20);

                string buttonCaption = String.Empty;
                if (plugin.activeState)
                {
                    buttonCaption = "Enabled";
                    GUI.contentColor = Color.green;
                }
                else
                {
                    buttonCaption = "Disabled";
                    GUI.contentColor = Color.red;
                }

                if (GUI.Button(enableButtonRect, buttonCaption))
                {
                    // toggle the state of the plugin by toggling it
                    plugin.activeState = !plugin.activeState;
                    
                    if (plugin.activeState)
                    {
                        plugin.initialize();
                        plugin.start(this);
                        
                        MonoSingleton<AudioManager>.Instance.PlaySound("ToggleOn");
                    }
                    else
                    {
                        plugin.activeState = false;
                        MonoSingleton<AudioManager>.Instance.PlaySound("ToggleOff");
                    }
                }

                GUI.contentColor = Color.white;

                index++;
            }

            // Mod Loader Log
            if (GUI.Button(new Rect(10, 30, 100, 20), "Mod-Log"))
            {
                if (gameObject.GetComponent<ModManagerLogWindow>() != null)
                {
                    Destroy(gameObject.GetComponent<ModManagerLogWindow>());
                }
                else
                {
                    gameObject.AddComponent<ModManagerLogWindow>().setup("mod_launcher.log");
                }
            }
            
            // Make the windows be draggable.
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
            
        }
        
    }
    
}