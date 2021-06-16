﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace GoingMedievalModLauncher.ui
{
    /**
     * The base class for user-interface windows.
     * This manages the id-, position-, size- and visibility handling,
     * as just the base-drawing functionalities. It also manages a simple close button.
     *
     * For rendering custom content inside a window, override the renderWindow method,
     * remember to call base.renderWindow() from the parent class.
     */
    public abstract class UIWindow : UIBehaviour
    {
        // an numeric value which is used to identify / differntiate between all windows 
        public int windowId;
        
        // the position and size of a window
        public Rect windowRect;
        
        // the visiblity of a window, true = shown, false = hidden
        public bool shown;

        // the alphanumeric title which is displayed on the top of the window
        public string windowTitle;

        // a position & size for the close button
        public Rect closeButtonRect = new Rect(0, 0, 20, 20);

        // the initial background color which is needed to draw every ui element, except the close button,
        // with a valid ui background
        public static Color initialBackgroundColor = Color.clear;

        // Manage the scrollView - scroll(Bar)Position, scrollView-Size and the max size of the rendered content
        private Vector2 scrollPosition = new Vector2();
        public Rect scrollViewRect = new Rect();
        public Rect scrollContentMaxSize = new Rect();

        private void Awake()
        {
            gameObject.AddComponent<RectTransform>();
        }

        private void renderWindow(int windowId)
        {
            
            // https://forum.unity.com/threads/how-to-use-a-scrollview-in-a-gui-window.92204/
            
            initialBackgroundColor = GUI.backgroundColor;
            
            GUI.contentColor = Color.white;
            GUI.backgroundColor = Color.clear;
            
            closeButtonRect.Set(windowRect.width - (closeButtonRect.width + 5), 1, 24, 16);

            if (GUI.Button(closeButtonRect, "X"))
            {
                shown = false;
            }

            GUI.backgroundColor = initialBackgroundColor;

            // enable the scroll view
            scrollPosition = GUI.BeginScrollView (scrollViewRect, scrollPosition,  scrollContentMaxSize);

            // draw the content of the inherited class
            renderContent();
            
            // disable the scroll view rendering
            GUI.EndScrollView();
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

        public abstract void renderContent();

    }
    
}