using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GoingMedievalModLauncher.ui
{
    public class ModManagerLogWindow : UIWindow
    {
        
        private StringBuilder logOutput = new StringBuilder();

        private string fileName = "mod_launcher.log";

        public void setup(string newFileName)
        {
            this.fileName = newFileName;
            
            // buildLog();
            logOutput.Clear();
            logOutput.Append(File.ReadAllText(this.fileName));
        }

        public void Start()
        {
            this.windowTitle = "Going Medieval - Log";
            this.windowId = 2;
            this.windowRect = new Rect(20, 450, 800, 400);
            this.shown = true;
            
            // buildLog();
            logOutput.Clear();
            logOutput.Append(File.ReadAllText(this.fileName));
        }

        public override void renderContent()
        {
            
            // setup the scrollView dimensions based on the amount of loaded mods
            scrollViewRect.Set(1,20, windowRect.width-4, windowRect.height-2);
            scrollContentMaxSize.Set(0,0, windowRect.width, 9999);
            
            GUI.Label(new Rect(15, 30, windowRect.width - 10, windowRect.height - 10), logOutput.ToString());

            // Make the windows be draggable.
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }


        public void buildLog()
        {
            // clear the log
            logOutput.Clear();

            logOutput.AppendLine();

            logOutput.Append("Game-Version: ");
            logOutput.Append(Application.version);
            logOutput.AppendLine();

            logOutput.Append("Unity-Version: ");
            logOutput.Append(Application.unityVersion);
            logOutput.AppendLine();

            logOutput.Append("Resolution: ");
            logOutput.Append(Screen.width);
            logOutput.Append(" x ");
            logOutput.Append(Screen.height);
            logOutput.Append(" @ ");
            logOutput.Append(Screen.dpi);
            logOutput.Append(" DPI");
            logOutput.AppendLine();

            logOutput.Append("Current level: ");
            logOutput.Append(Application.loadedLevelName);
            logOutput.AppendLine();

            logOutput.Append("Mouse-Position: ");
            logOutput.Append(Input.mousePosition.ToString());
            logOutput.AppendLine();

            logOutput.Append("Real time:");
            logOutput.Append(DateTime.Now.ToString("dd.MM.yyyy - HH:mm:ss"));
            logOutput.AppendLine();

            logOutput.Append("Frames per second:");
            logOutput.Append((int) (1.0f / Time.smoothDeltaTime));
            logOutput.AppendLine();
        }
    }
}