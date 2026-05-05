using MarioGame.Components.Gameplay;
using MarioGame.Components.Gameplay.Pickups;
using MarioGame.Levels;
using MarioGame.Scenes.GameFlow;
using UnityEngine;
using UnityEngine.UI;
using MarioGame.Utils.Runtime;

namespace MarioGame.Components.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas _canvas;
        private Text _topLeftText;
        private GameObject _pausePanel;
        private GameObject _gameOverPanel;
        private GameObject _levelCompletePanel;
        private Text _levelCompleteText;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildUi();
        }

        private void Update()
        {
            var runtime = LevelRuntime.Current;
            if (runtime == null || _topLeftText == null)
                return;

            var coins = CoinManager.Instance != null ? CoinManager.Instance.Coins : 0;
            var totalCoins = CoinManager.Instance != null ? CoinManager.Instance.TotalCoins : 0;
            var lives = runtime.PlayerHealth != null ? runtime.PlayerHealth.Lives : 0;
            var theme = WorldTheme.ForLevel(runtime.LevelNumber);

            _topLeftText.text =
                $"Level {runtime.LevelNumber}/100 ({theme})\n" +
                $"Coins {coins}/{totalCoins}   Time {runtime.ElapsedTime:0.0}s\n" +
                $"Lives {lives}";
        }

        public void ShowPause(bool show)
        {
            if (_pausePanel != null)
                _pausePanel.SetActive(show);
        }

        public void ShowGameOver(bool show)
        {
            if (_gameOverPanel != null)
                _gameOverPanel.SetActive(show);
        }

        public void ShowLevelComplete(bool show, int stars = 0, int coins = 0, int totalCoins = 0, float timeSeconds = 0f)
        {
            if (_levelCompletePanel != null)
                _levelCompletePanel.SetActive(show);

            if (!show || _levelCompleteText == null)
                return;

            _levelCompleteText.text =
                $"Stars: {stars} / 3\n" +
                $"Coins: {coins}/{totalCoins}\n" +
                $"Time: {timeSeconds:0.0}s";
        }

        private void BuildUi()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = gameObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1f;
            gameObject.AddComponent<GraphicRaycaster>();

            _topLeftText = CreateText("HUDText", new Vector2(18, -16), anchorTopLeft: true, fontSize: 26);
            _topLeftText.alignment = TextAnchor.UpperLeft;
            var hudShadow = _topLeftText.gameObject.AddComponent<Shadow>();
            hudShadow.effectColor = new Color(0f, 0f, 0f, 0.5f);
            hudShadow.effectDistance = new Vector2(1.5f, -1.5f);

            _pausePanel = CreatePanel("PausePanel", new Color(0f, 0f, 0f, 0.62f));
            _pausePanel.SetActive(false);
            CreateCenteredLabel(_pausePanel.transform, "PAUSED", 46);
            CreateCenteredButton(_pausePanel.transform, "Resume", y: -40, onClick: () => GameManager.Instance?.TogglePause());
            CreateCenteredButton(_pausePanel.transform, "Restart", y: -120, onClick: () => GameManager.Instance?.RestartLevel());
            CreateCenteredButton(_pausePanel.transform, "Music", y: -200, onClick: () => GameManager.Instance?.ToggleMusic());
            CreateCenteredButton(_pausePanel.transform, "SFX", y: -280, onClick: () => GameManager.Instance?.ToggleSfx());
            CreateCenteredButton(_pausePanel.transform, "Shop", y: -360, onClick: () => GameManager.Instance?.ToggleShop());
            CreateCenteredButton(_pausePanel.transform, "Quit", y: -440, onClick: () => GameManager.Instance?.LoadLevel(1));

            _gameOverPanel = CreatePanel("GameOverPanel", new Color(0f, 0f, 0f, 0.72f));
            _gameOverPanel.SetActive(false);
            CreateCenteredLabel(_gameOverPanel.transform, "GAME OVER", 54);
            CreateCenteredButton(_gameOverPanel.transform, "Restart", y: -80, onClick: () => GameManager.Instance?.RestartLevel());
            CreateCenteredButton(_gameOverPanel.transform, "Level Select", y: -160, onClick: () => GameManager.Instance?.ToggleLevelSelect());

            _levelCompletePanel = CreatePanel("LevelCompletePanel", new Color(0f, 0f, 0f, 0.72f));
            _levelCompletePanel.SetActive(false);
            CreateCenteredLabel(_levelCompletePanel.transform, "LEVEL COMPLETE", 50);
            _levelCompleteText = CreateCenteredInfo(_levelCompletePanel.transform, y: 10);
            CreateCenteredButton(_levelCompletePanel.transform, "Next", y: -80, onClick: () =>
            {
                var next = LevelRuntime.Current != null ? Mathf.Min(LevelRuntime.Current.LevelNumber + 1, 100) : 1;
                var ads = MarioGame.Components.Monetization.Ads.AdsManager.Instance;
                if (ads != null && ads.ShouldShowInterstitial())
                {
                    ads.ShowInterstitial("level_complete_next", onClosed: () => GameManager.Instance?.LoadLevel(next));
                }
                else
                {
                    GameManager.Instance?.LoadLevel(next);
                }
            });
            CreateCenteredButton(_levelCompletePanel.transform, "Retry", y: -160, onClick: () => GameManager.Instance?.RestartLevel());
            CreateCenteredButton(_levelCompletePanel.transform, "Levels", y: -240, onClick: () => GameManager.Instance?.ToggleLevelSelect());
        }

        private Text CreateText(string name, Vector2 anchoredPosition, bool anchorTopLeft, int fontSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var rect = go.AddComponent<RectTransform>();
            if (anchorTopLeft)
            {
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
            }

            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = new Vector2(900, 160);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = fontSize;
            t.color = Color.white;
            return t;
        }

        private GameObject CreatePanel(string name, Color bg)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var img = go.AddComponent<Image>();
            img.sprite = RuntimeUiSprites.RoundedRect;
            img.type = Image.Type.Sliced;
            img.color = bg;
            return go;
        }

        private void CreateCenteredLabel(Transform parent, string text, int size)
        {
            var go = new GameObject("Label");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.65f);
            rect.anchorMax = new Vector2(0.5f, 0.65f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(700, 90);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = size;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.text = text;
        }

        private Text CreateCenteredInfo(Transform parent, float y)
        {
            var go = new GameObject("Info");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, y);
            rect.sizeDelta = new Vector2(700, 180);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 32;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            var shadow = go.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);
            return t;
        }

        private void CreateCenteredButton(Transform parent, string label, float y, System.Action onClick)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(0, y);
            rect.sizeDelta = new Vector2(300, 70);

            var img = go.AddComponent<Image>();
            img.sprite = RuntimeUiSprites.RoundedRect;
            img.type = Image.Type.Sliced;
            img.color = new Color(1f, 1f, 1f, 0.12f);
            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(() => onClick?.Invoke());

            var txt = new GameObject("Text");
            txt.transform.SetParent(go.transform, false);
            var tr = txt.AddComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;
            var t = txt.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.alignment = TextAnchor.MiddleCenter;
            t.fontSize = 28;
            t.color = Color.white;
            t.text = label;

            var shadow = txt.AddComponent<Shadow>();
            shadow.effectColor = new Color(0f, 0f, 0f, 0.45f);
            shadow.effectDistance = new Vector2(1.5f, -1.5f);
        }
    }
}

