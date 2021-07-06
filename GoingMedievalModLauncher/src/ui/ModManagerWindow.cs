using GoingMedievalModLauncher.plugins;
using NSEipix.Base;
using NSMedieval.Sound;
using NSMedieval.UI;
using UnityEngine;

namespace GoingMedievalModLauncher.ui
{
    public class ModManagerWindow : UIWindow
    {

        private static GUIStyle bigFontSizeStyle = new GUIStyle();
        
        public ModManagerWindow()
        {
            // BlackBarMessageController.Instance.ShowClickableBlackBarMessage("Hello World!", Vector3.one);
        }

        public new void Start()
        {
            this.windowTitle = "Going Medieval - Mod Manager";
            this.windowId = 1;
            this.windowRect = new Rect(10, 10, 800, 600);
            this.shown = true;

            bigFontSizeStyle.fontSize = 16;
            bigFontSizeStyle.normal.textColor = Color.white;
            
            base.Start();
            
        }

        private void Update()
        {
            
            if (Input.GetKeyDown(KeyCode.F12))
            {
                // toggle the visibility via key-press
                shown = !shown;
            }
            
        }

        public override void RenderContent()
        {

            // render a table-like structure
            int index = 0;
            int sizePerRow = 60;
            
            // setup the scrollView dimensions based on the amount of loaded mods
            scrollViewRect.Set(1,20, windowRect.width-4, windowRect.height-2);
            scrollContentMaxSize.Set(0,0, windowRect.width, 
                height: 150 + (Singleton<PluginManager>.Instance.GetPlugins().Count * sizePerRow)    
            );
            
            // Toggle the visibility of the mod-loader log
            if (GUI.Button(new Rect(10, 10, 100, 20), "Mod-Log"))
            {
                var modManagerLogWindow = gameObject.GetComponent<ModManagerLogWindow>();

                if (modManagerLogWindow == null) gameObject.AddComponent<ModManagerLogWindow>();

                modManagerLogWindow.shown = !modManagerLogWindow.shown;
                
                if(modManagerLogWindow.shown) modManagerLogWindow.setup("mod_launcher.log");
            }
            
            // Render a simple note for using the mod-loader manager
            Rect usageLabelRect = new Rect(150, 10, 250, 30);
            GUI.Label(usageLabelRect, "Toggle the Mod-Manager with F12.", bigFontSizeStyle);
            
            // draw a line below the mod-log button and usage-note
            drawRect(new Rect(0, 40, windowRect.width , 2), lineColor );
            
            // render a row per mod on the manager ui
            foreach (var pluginc in Singleton<PluginManager>.Instance.GetPlugins())
            {
                // setup the positions for labels
                var y = 50 + (index * sizePerRow);
                var nameLabelRect = new Rect(10, y, 300, 20);
                var descriptionLabelRect = new Rect(10, y + 14, windowRect.width - 150, 40);

                // render the labels name, description and version of a mod
                GUI.Label(nameLabelRect, pluginc.Name + ", " + pluginc.Version, bigFontSizeStyle);
                GUI.Label(descriptionLabelRect, pluginc.Description);

                // setup a toggle-state button for enabling / disabling a mod
                var enableButtonRect = new Rect(windowRect.width - 150, y, 100, 40);
                string buttonCaption = pluginc.ActiveState ? "Enabled" : "Disabled";
                GUI.contentColor = pluginc.ActiveState ? Color.green : Color.red;

                // Setup the toggle-state button
                if (GUI.Button(enableButtonRect, buttonCaption))
                {
                    // toggle the state of the plugin by toggling it
                    pluginc.ActiveState = !pluginc.ActiveState;
                    
                    if (pluginc.ActiveState)
                    {
                        Logger.Instance.info("Enabling plugin \"" + pluginc.Name + "\" ...");
                        pluginc.plugin.initialize();
                        pluginc.plugin.start(this);
                        
                        MonoSingleton<AudioManager>.Instance.PlaySound("ToggleOn");
                    }
                    else
                    {
                        Logger.Instance.info("Disabling plugin \"" + pluginc.Name + "\" ...");

                        pluginc.plugin.disable(this);
                        pluginc.ActiveState = false;
                        MonoSingleton<AudioManager>.Instance.PlaySound("ToggleOff");
                    }
                }
                
                // revert the style to anything before
                GUI.contentColor = Color.white;
                
                index++;
                
                // draw a line below every entry
                drawRect(new Rect(0,40 + index * sizePerRow, windowRect.width, 2), lineColor );
            }
 
            // Make the windows be draggable.
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
            
        }
    
        // this is required for drawing labels without adding additional assets 
        private static Texture2D staticRectTexture;
        private static GUIStyle staticRectStyle;
        private static Color lineColor = Color.black;
 
        // Note that this function is only meant to be called from OnGUI() functions.
        public static void drawRect( Rect position, Color color )
        {
            if( staticRectTexture == null )
            {
                staticRectTexture = new Texture2D( 1, 1 );
            }
 
            if( staticRectStyle == null )
            {
                staticRectStyle = new GUIStyle();
            }
 
            staticRectTexture.SetPixel( 0, 0, color );
            staticRectTexture.Apply();
 
            staticRectStyle.normal.background = staticRectTexture;
 
            GUI.Box( position, GUIContent.none, staticRectStyle );
 
 
        }
        
    }
    
}