using MarioGame.Components.Gameplay;
using MarioGame.Components.Gameplay.Pickups;
using MarioGame.Levels;
using MarioGame.Scenes.GameFlow;
using UnityEngine;
using UnityEngine.UI;

namespace MarioGame.Components.UI
{
    public sealed class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        private Canvas _canvas;
        private Text _topLeftText;
        private GameObject _pausePanel;
        private GameObject _gameOverPanel;

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

        private void BuildUi()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();

            _topLeftText = CreateText("HUDText", new Vector2(18, -16), anchorTopLeft: true, fontSize: 26);
            _topLeftText.alignment = TextAnchor.UpperLeft;

            _pausePanel = CreatePanel("PausePanel", new Color(0f, 0f, 0f, 0.62f));
            _pausePanel.SetActive(false);
            CreateCenteredLabel(_pausePanel.transform, "PAUSED", 46);
            CreateCenteredButton(_pausePanel.transform, "Resume", y: -40, onClick: () => GameManager.Instance?.TogglePause());
            CreateCenteredButton(_pausePanel.transform, "Restart", y: -120, onClick: () => GameManager.Instance?.RestartLevel());
            CreateCenteredButton(_pausePanel.transform, "Quit", y: -200, onClick: () => GameManager.Instance?.LoadLevel(1));

            _gameOverPanel = CreatePanel("GameOverPanel", new Color(0f, 0f, 0f, 0.72f));
            _gameOverPanel.SetActive(false);
            CreateCenteredLabel(_gameOverPanel.transform, "GAME OVER", 54);
            CreateCenteredButton(_gameOverPanel.transform, "Restart", y: -80, onClick: () => GameManager.Instance?.RestartLevel());
            CreateCenteredButton(_gameOverPanel.transform, "Level Select", y: -160, onClick: () => GameManager.Instance?.ToggleLevelSelect());
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
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
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
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.fontSize = size;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.text = text;
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

            go.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.12f);
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
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.alignment = TextAnchor.MiddleCenter;
            t.fontSize = 28;
            t.color = Color.white;
            t.text = label;
        }
    }
}
