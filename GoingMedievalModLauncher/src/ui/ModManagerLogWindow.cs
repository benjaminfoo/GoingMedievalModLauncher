using System.Text;
using UnityEngine;


namespace GoingMedievalModLauncher.ui
{
    public class ModManagerLogWindow : UIWindow
    {

        private StringBuilder logOutput = new StringBuilder();

        private string fileName = "mod_launcher.log";

        private float calculatedMaxWindowHeight = 16;
        
        public void setup(string newFileName)
        {
            this.fileName = newFileName;
            logOutput.Clear();
            logOutput.Append(Logger.Instance.GetCurrentLogs());
            Logger.Instance.OnFlushing += updateText;

                // count the number of newlines from the log and multiply it by the line height 
            calculatedMaxWindowHeight = logOutput.ToString().Split('\n').Length * 16+32;
        }

        private void updateText(string s)
        {
            if(s == null)
                return;
            logOutput.Append(s);
            calculatedMaxWindowHeight = logOutput.ToString().Split('\n').Length * 16+32;
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Logger.Instance.OnFlushing -= updateText;
        }


    }
}