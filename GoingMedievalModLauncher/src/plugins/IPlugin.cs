using UnityEngine;

namespace GoingMedievalModLauncher
{
    public interface IPlugin
    {
        // A alphanumeric string which describes the name of the plugin / mod.
        string Name { get; }

        // A alphanumeric string which describes the functionality of the plugin / mod.
        string Description { get;  }

        // A alphanumeric string which describes the version of the plugin / mod.
        string Version { get; }

        // a boolean variable which indicates that this mod is active or not
        bool activeState { get; set; }

        void initialize();

        /**
         * The start-method is called once after the corresponding gameObject has been enabled.
         * For more details, see: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
         */
        void start(MonoBehaviour root);

        /**
         * The update-method is called every frame.
         * For more details, see:https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
         */
        void update(MonoBehaviour root);

        /* 
         * The disable-method is called by the user or at the end of the game (quitting the application)
         * This can be used for final operations before the application gets shutdown.
         */
        void disable(MonoBehaviour root);

    }
}