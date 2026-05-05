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
        private MarioGame.Components.Monetization.IAP.IapManager _iap;
        private MarioGame.Components.Monetization.Shop.ShopController _shop;

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
            EnsureAds();
            EnsureCoinManager();
            EnsureUi();
            EnsureRuntime();

            AudioService.SetMusicEnabled(_progression.MusicEnabled);
            AudioService.SetSfxEnabled(_progression.SfxEnabled);

            EnsureIap();
            EnsureShop();
            _runtime.LoadLevel(_progression.UnlockedLevel);
        }

        private void EnsureAudio()
        {
            if (Object.FindAnyObjectByType<AudioService>() != null)
                return;

            var go = new GameObject("AudioService");
            DontDestroyOnLoad(go);
            go.AddComponent<AudioService>();
        }

        private void EnsureAds()
        {
            if (Object.FindAnyObjectByType<MarioGame.Components.Monetization.Ads.AdsManager>() != null)
                return;

            var go = new GameObject("AdsManager");
            DontDestroyOnLoad(go);
            go.AddComponent<MarioGame.Components.Monetization.Ads.AdsManager>();
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

            _runtime = Object.FindAnyObjectByType<LevelRuntime>();
            if (_runtime == null)
            {
                var go = new GameObject("LevelRuntime");
                DontDestroyOnLoad(go);
                _runtime = go.AddComponent<LevelRuntime>();
            }

            _runtime.Construct(_progression);
        }

        private void EnsureIap()
        {
            if (_iap != null)
                return;

            _iap = Object.FindAnyObjectByType<MarioGame.Components.Monetization.IAP.IapManager>();
            if (_iap == null)
            {
                var go = new GameObject("IapManager");
                DontDestroyOnLoad(go);
                _iap = go.AddComponent<MarioGame.Components.Monetization.IAP.IapManager>();
            }

            _iap.Construct(_saveService, _progression);
        }

        private void EnsureShop()
        {
            if (_shop != null)
                return;

            _shop = Object.FindAnyObjectByType<MarioGame.Components.Monetization.Shop.ShopController>();
            if (_shop == null)
            {
                var go = new GameObject("Shop");
                DontDestroyOnLoad(go);
                _shop = go.AddComponent<MarioGame.Components.Monetization.Shop.ShopController>();
            }

            _shop.Construct(_saveService);
        }

        public void ToggleShop()
        {
            if (_shop == null)
                return;

            if (_shop.IsOpen)
            {
                _shop.Toggle(false);
                SetPaused(false);
                return;
            }

            _shop.Toggle(true);
            SetPaused(true);
            UIManager.Instance?.ShowPause(false);
        }

        public void LoadLevel(int levelNumber)
        {
            _paused = false;
            Time.timeScale = 1f;
            UIManager.Instance?.ShowPause(false);
            UIManager.Instance?.ShowGameOver(false);
            UIManager.Instance?.ShowLevelComplete(false);
            _runtime?.LoadLevel(levelNumber);
        }
        public void RestartLevel()
        {
            _paused = false;
            Time.timeScale = 1f;
            UIManager.Instance?.ShowPause(false);
            UIManager.Instance?.ShowGameOver(false);
            UIManager.Instance?.ShowLevelComplete(false);
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

        public void ToggleMusic()
        {
            if (_progression == null) return;
            _progression.MusicEnabled = !_progression.MusicEnabled;
            AudioService.SetMusicEnabled(_progression.MusicEnabled);
        }

        public void ToggleSfx()
        {
            if (_progression == null) return;
            _progression.SfxEnabled = !_progression.SfxEnabled;
            AudioService.SetSfxEnabled(_progression.SfxEnabled);
        }

        public bool MusicEnabled => _progression != null && _progression.MusicEnabled;
        public bool SfxEnabled => _progression != null && _progression.SfxEnabled;
    }
}

