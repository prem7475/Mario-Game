using MarioGame.Components.Camera;
using MarioGame.Components.Input;
using MarioGame.Components.Player;
using UnityEngine;

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
            var cam = camGo.AddComponent<UnityEngine.Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 6.5f;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = new Color(0.55f, 0.8f, 1f);
            camGo.AddComponent<AudioListener>();
        }

        private void EnsureUIAndInput()
        {
            if (FindObjectOfType<VirtualControlsHUD>() != null)
                return;

            var hud = new GameObject("VirtualControlsHUD");
            DontDestroyOnLoad(hud);
            hud.AddComponent<VirtualControlsHUD>();

            var input = new GameObject("InputService");
            DontDestroyOnLoad(input);
            input.AddComponent<InputService>();
        }

        private void EnsureGameManager()
        {
            if (FindObjectOfType<GameManager>() != null)
                return;

            var go = new GameObject("GameManager");
            DontDestroyOnLoad(go);
            go.AddComponent<GameManager>();
        }
    }
}
