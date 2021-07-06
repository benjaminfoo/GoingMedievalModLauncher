using UnityEngine;

namespace GoingMedievalModLauncher.plugins
{
    public interface IPlugin
    {

     /// <summary>
        /// The plugins initalization phase
        /// </summary>
        void initialize();

        /** <summary>
         * The start-method is called once after the corresponding gameObject has been enabled.
         * For more details, see: https://docs.unity3d.com/ScriptReference/MonoBehaviour.Start.html
         * </summary>
          */
        void start(MonoBehaviour root);

        /**
         * <summary>
         * The update-method is called every frame.
         * For more details, see:https://docs.unity3d.com/ScriptReference/MonoBehaviour.Update.html
         * </summary>
          */
        void update(MonoBehaviour root);

        /*
         * <summary>
         * The disable-method is called by the user or at the end of the game (quitting the application)
         * This can be used for final operations before the application gets shutdown.
         * </summary>
         */
        void disable(MonoBehaviour root);

    }
}