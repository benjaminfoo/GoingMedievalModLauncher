using System;
using System.IO;
using System.Reflection;
using System.Text;
using NSMedieval.Model;
using NSMedieval.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GoingMedievalModLauncher.ui
{
    public class ModManagerLogWindow : UIWindow
    {

        private static Sprite sp = Resources.GetBuiltinResource<Sprite>("frame_01");
        
        private StringBuilder logOutput = new StringBuilder();

        private string fileName = "mod_launcher.log";

        private float calculatedMaxWindowHeight;
        
        public void setup(string newFileName)
        {
            this.fileName = newFileName;
            
            // buildLog();
            logOutput.Clear();
            logOutput.Append(File.ReadAllText(this.fileName));

            // count the number of newlines from the log and multiply it by the line height 
            calculatedMaxWindowHeight = logOutput.ToString().Split('\n').Length * 16;
        }

        public new void Start()
        {
            this.windowTitle = "Going Medieval - Log";
            this.windowId = 2;
            this.windowRect = new Rect(20, 450, 800, 400);
            this.shown = true;
            
            setup(this.fileName);
        }

        public override void RenderContent()
        {
            
            // setup the scrollView dimensions based on the amount of loaded mods
            scrollViewRect.Set(1,20, windowRect.width-4, windowRect.height-2);
            scrollContentMaxSize.Set(0,0, windowRect.width, calculatedMaxWindowHeight);
            
            GUI.Label(new Rect(15, 30, windowRect.width - 10, calculatedMaxWindowHeight), logOutput.ToString());

            // Make the windows be draggable.
            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
        

    }
}