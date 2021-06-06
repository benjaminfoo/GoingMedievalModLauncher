using UnityEngine;

namespace GoingMedievalModLauncher
{
    public interface IPlugin
    {
        string Name { get; }
        string Version { get; }

        void initialize();

        void start(MonoBehaviour root);

        void update(MonoBehaviour root);

    }
}