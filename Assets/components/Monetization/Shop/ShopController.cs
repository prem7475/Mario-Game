using MarioGame.Components.Gameplay;
using MarioGame.Components.Monetization.Ads;
using MarioGame.Components.Monetization.IAP;
using MarioGame.Utils.Save;
using UnityEngine;
using UnityEngine.UI;

namespace MarioGame.Components.Monetization.Shop
{
    public sealed class ShopController : MonoBehaviour
    {
        public static ShopController Instance { get; private set; }

        private Canvas _canvas;
        private GameObject _panel;
        private Text _status;
        private SaveService _save;

        public bool IsOpen => _panel != null && _panel.activeSelf;

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

        public void Construct(SaveService save)
        {
            _save = save;
            if (IapManager.Instance != null)
            {
                IapManager.Instance.OnPurchaseSucceeded += _ => Refresh();
                IapManager.Instance.OnPurchaseFailed += (_, __) => Refresh();
            }
        }

        public void Toggle(bool show)
        {
            if (_panel != null)
                _panel.SetActive(show);
            if (show)
                Refresh();
        }

        public void Toggle()
        {
            Toggle(!IsOpen);
        }

        public void Refresh()
        {
            if (_status == null || _save == null)
                return;

            _status.text =
                $"Soft Coins: {_save.Data.softCoins}\n" +
                $"Extra Lives: {_save.Data.inventoryExtraLives}\n" +
                $"Unlock All: {(_save.Data.purchasedUnlockAllLevels ? "YES" : "NO")}";
        }

        private void BuildUi()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();

            _panel = new GameObject("ShopPanel");
            _panel.transform.SetParent(transform, false);
            var rect = _panel.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            _panel.AddComponent<Image>().color = new Color(0, 0, 0, 0.72f);
            _panel.SetActive(false);

            var title = CreateText(_panel.transform, "SHOP", new Vector2(0, 320), 54);
            title.alignment = TextAnchor.MiddleCenter;

            _status = CreateText(_panel.transform, "Status", new Vector2(0, 160), 32);
            _status.alignment = TextAnchor.MiddleCenter;

            CreateButton(_panel.transform, "Reward: +1 Life", new Vector2(0, 40), () =>
            {
                AdsManager.Instance?.ShowRewarded("shop_life", onReward: () =>
                {
                    var runtime = MarioGame.Levels.LevelRuntime.Current;
                    runtime?.PlayerHealth?.AddLife(1);
                }, onClosed: Refresh);
            });

            CreateButton(_panel.transform, "Use 1 Extra Life", new Vector2(0, -10), () =>
            {
                if (_save != null && _save.Data.inventoryExtraLives > 0)
                {
                    _save.Data.inventoryExtraLives -= 1;
                    _save.Save();
                    var runtime = MarioGame.Levels.LevelRuntime.Current;
                    runtime?.PlayerHealth?.AddLife(1);
                }
                Refresh();
            });

            CreateButton(_panel.transform, "Buy 5 Lives", new Vector2(0, -60), () =>
            {
                IapManager.Instance?.Purchase(ProductIds.ExtraLivesSmall);
                Refresh();
            });

            CreateButton(_panel.transform, "Buy 500 Coins", new Vector2(0, -160), () =>
            {
                IapManager.Instance?.Purchase(ProductIds.CoinPackSmall);
                Refresh();
            });

            CreateButton(_panel.transform, "Unlock All Levels", new Vector2(0, -260), () =>
            {
                IapManager.Instance?.Purchase(ProductIds.UnlockAllLevels);
                Refresh();
            });

            CreateButton(_panel.transform, "Close", new Vector2(0, -380), () =>
            {
                Toggle(false);
                MarioGame.Scenes.GameFlow.GameManager.Instance?.SetPaused(false);
            });
        }

        private static Text CreateText(Transform parent, string text, Vector2 pos, int size)
        {
            var go = new GameObject("Text_" + text);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(900, 120);
            var t = go.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = size;
            t.color = Color.white;
            t.text = text;
            return t;
        }

        private static void CreateButton(Transform parent, string label, Vector2 pos, System.Action onClick)
        {
            var go = new GameObject("Btn_" + label);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = pos;
            rect.sizeDelta = new Vector2(520, 84);
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
            t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            t.fontSize = 30;
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;
            t.text = label;
        }
    }
}

