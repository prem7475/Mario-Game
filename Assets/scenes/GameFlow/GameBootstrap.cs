using MarioGame.Components.Camera;
using MarioGame.Components.Input;
using MarioGame.Components.Player;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MarioGame.Scenes.GameFlow
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        private void Awake()
        {
            Application.targetFrameRate = 60;

            EnsureCamera();
            EnsureUIAndInput();
            EnsureGameManager();
        }

        private void EnsureCamera()
        {
            if (Camera.main != null)
                return;

            var camGo = new GameObject("Main Camera");
            camGo.tag = "MainCamera";
            camGo.transform.position = new Vector3(0f, 0f, -10f);
            var cam = camGo.AddComponent<UnityEngine.Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6.5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.55f, 0.8f, 1f);
            camGo.AddComponent<AudioListener>();
        }

        private void EnsureUIAndInput()
        {
            if (Object.FindAnyObjectByType<VirtualControlsHUD>() == null)
            {
                var hud = new GameObject("VirtualControlsHUD");
                DontDestroyOnLoad(hud);
                hud.AddComponent<VirtualControlsHUD>();
            }

            if (Object.FindAnyObjectByType<InputService>() == null)
            {
                var input = new GameObject("InputService");
                DontDestroyOnLoad(input);
                input.AddComponent<InputService>();
            }

            EnsureEventSystem();
        }

        private void EnsureEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
                return;

            var eventSystem = new GameObject("EventSystem");
            DontDestroyOnLoad(eventSystem);
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private void EnsureGameManager()
        {
            if (Object.FindAnyObjectByType<GameManager>() != null)
                return;

            var go = new GameObject("GameManager");
            DontDestroyOnLoad(go);
            go.AddComponent<GameManager>();
        }
    }
}

