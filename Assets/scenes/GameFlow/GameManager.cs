using MarioGame.Components.Audio;
using MarioGame.Components.Gameplay.Pickups;
using MarioGame.Components.UI;
using MarioGame.Levels;
using MarioGame.Utils.Save;
using UnityEngine;

namespace MarioGame.Scenes.GameFlow
{
    public sealed class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        private SaveService _saveService;
        private LevelProgression _progression;
        private LevelRuntime _runtime;
        private bool _paused;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _saveService = new SaveService();
            _progression = new LevelProgression(_saveService);

            EnsureAudio();
            EnsureCoinManager();
            EnsureUi();
            EnsureRuntime();

            AudioService.SetSoundEnabled(_progression.SoundEnabled);
            _runtime.LoadLevel(_progression.UnlockedLevel);
        }

        private void EnsureAudio()
        {
            if (FindObjectOfType<AudioService>() != null)
                return;

            var go = new GameObject("AudioService");
            DontDestroyOnLoad(go);
            go.AddComponent<AudioService>();
        }

        private void EnsureCoinManager()
        {
            if (CoinManager.Instance != null)
                return;

            var go = new GameObject("CoinManager");
            DontDestroyOnLoad(go);
            go.AddComponent<CoinManager>();
        }

        private void EnsureUi()
        {
            if (UIManager.Instance != null)
                return;

            var go = new GameObject("UIManager");
            DontDestroyOnLoad(go);
            go.AddComponent<UIManager>();
        }

        private void EnsureRuntime()
        {
            if (_runtime != null)
                return;

            _runtime = FindObjectOfType<LevelRuntime>();
            if (_runtime == null)
            {
                var go = new GameObject("LevelRuntime");
                DontDestroyOnLoad(go);
                _runtime = go.AddComponent<LevelRuntime>();
            }

            _runtime.Construct(_progression);
        }

        public void LoadLevel(int levelNumber)
        {
            _paused = false;
            Time.timeScale = 1f;
            UIManager.Instance?.ShowPause(false);
            UIManager.Instance?.ShowGameOver(false);
            _runtime?.LoadLevel(levelNumber);
        }
        public void RestartLevel()
        {
            _paused = false;
            Time.timeScale = 1f;
            UIManager.Instance?.ShowPause(false);
            UIManager.Instance?.ShowGameOver(false);
            _runtime?.RestartLevel();
        }

        public void TogglePause()
        {
            _paused = !_paused;
            Time.timeScale = _paused ? 0f : 1f;
            UIManager.Instance?.ShowPause(_paused);
        }

        public void SetPaused(bool paused)
        {
            _paused = paused;
            Time.timeScale = _paused ? 0f : 1f;
            UIManager.Instance?.ShowPause(_paused);
        }

        public void ToggleLevelSelect()
        {
            _runtime?.ToggleLevelSelect();
        }
    }
}
