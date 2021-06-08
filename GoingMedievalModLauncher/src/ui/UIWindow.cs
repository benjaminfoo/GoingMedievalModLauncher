using UnityEngine;

namespace GoingMedievalModLauncher.ui
{
    public abstract class UIWindow : MonoBehaviour
    {
        
        public int windowId;
        public Rect windowRect;
        public bool shown;

        public string windowTitle;

        private Rect closeButtonRect = new Rect(0, 0, 20, 20);

        public void renderWindow(int windowId)
        {
            GUI.contentColor = Color.white;
            
            closeButtonRect.Set(2 + windowRect.width - (closeButtonRect.width + 5), 2, 24, 24);
            if (GUI.Button(closeButtonRect, "X"))
            {
                shown = false;
            }
            
        }
    }
    
}