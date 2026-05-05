using UnityEngine;
using UnityEngine.EventSystems;

namespace MarioGame.Utils.Runtime
{
    public static class Bootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Init()
        {
            if (Object.FindAnyObjectByType<BootMarker>() != null)
                return;

            var marker = new GameObject("Boot");
            marker.AddComponent<BootMarker>();

            if (Object.FindAnyObjectByType<EventSystem>() == null)
            {
                var es = new GameObject("EventSystem");
                es.AddComponent<EventSystem>();
                es.AddComponent<StandaloneInputModule>();
            }

            marker.AddComponent<MarioGame.Scenes.GameFlow.GameBootstrap>();
        }

        private sealed class BootMarker : MonoBehaviour { }
    }
}

