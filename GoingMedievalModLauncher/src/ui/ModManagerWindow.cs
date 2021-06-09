using System;
using System.Text;
using HarmonyLib;
using NSEipix;
using NSEipix.Base;
using NSEipix.Repository;
using NSMedieval;
using NSMedieval.Components.Base;
using NSMedieval.Construction;
using NSMedieval.DevConsole;
using NSMedieval.Dictionary;
using NSMedieval.Enums;
using NSMedieval.GameEventSystem;
using NSMedieval.InfoMessages;
using NSMedieval.Model;
using NSMedieval.Model.MapNew;
using NSMedieval.Repository;
using NSMedieval.Sound;
using NSMedieval.Tools;
using NSMedieval.Tools.Debug;
using NSMedieval.Types;
using NSMedieval.UI;
using NSMedieval.UI.Utils;
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

        public override void renderContent()
        {
            int index = 1;
            int offsetY = 30;
            
            // render a row per mod on the manager ui
            foreach (var plugin in PluginManager.getInstance().GetPlugins())
            {
                // setup the positions for labels
                var nameLabelRect = new Rect(10, 40 + (index * offsetY), 150, 20);
                var versionLabelRect = new Rect(350, 40 + (index * offsetY), 100, 20);
                var descriptionLabelRect = new Rect(150, 40 + (index * offsetY), 190, 20);

                // render the labels name, description and version of a mod
                GUI.Label(nameLabelRect, plugin.Name);
                GUI.Label(descriptionLabelRect, plugin.Description);
                GUI.Label(versionLabelRect, plugin.Version);


                // setup a toggle-state button for enabling / disabling a mod
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
                        plugin.disable(this);
                        plugin.activeState = false;
                        MonoSingleton<AudioManager>.Instance.PlaySound("ToggleOff");
                    }
                }

                GUI.contentColor = Color.white;

                index++;
            }

            // Toggle the visibility of the mod-loader log
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
            
            // Render a simple note for using the mod-loader manager
            Rect usageLabelRect = new Rect(10, windowRect.height - 40, 200, 40);
            GUI.Label(usageLabelRect, "Toggle the Mod-Manager with F1.");
            
            // Make the windows be draggable.
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
            
        }
        
    }
    
}