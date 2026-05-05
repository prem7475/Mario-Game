using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MarioGame.Components.Input
{
    public sealed class VirtualControlsHUD : MonoBehaviour
    {
        private Canvas _canvas;

        private void Awake()
        {
            _canvas = gameObject.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameObject.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            gameObject.AddComponent<GraphicRaycaster>();

            CreateButton("Left", new Vector2(110, 110), new Vector2(140, 140),
                down: () => VirtualInput.PressLeft(true),
                up: () => VirtualInput.PressLeft(false));

            CreateButton("Right", new Vector2(280, 110), new Vector2(140, 140),
                down: () => VirtualInput.PressRight(true),
                up: () => VirtualInput.PressRight(false));

            CreateButton("Jump", new Vector2(-160, 120), new Vector2(170, 170),
                down: () => VirtualInput.PressJump(true),
                up: () => VirtualInput.PressJump(false),
                anchorRight: true);
        }

        private void CreateButton(
            string label,
            Vector2 anchoredPosition,
            Vector2 size,
            System.Action down,
            System.Action up,
            bool anchorRight = false)
        {
            var go = new GameObject(label);
            go.transform.SetParent(transform, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            if (anchorRight)
            {
                rect.anchorMin = new Vector2(1, 0);
                rect.anchorMax = new Vector2(1, 0);
                rect.pivot = new Vector2(1, 0);
            }
            else
            {
                rect.anchorMin = new Vector2(0, 0);
                rect.anchorMax = new Vector2(0, 0);
                rect.pivot = new Vector2(0, 0);
            }

            rect.anchoredPosition = anchoredPosition;

            var image = go.AddComponent<Image>();
            image.color = new Color(1f, 1f, 1f, 0.18f);

            var btnText = new GameObject("Text");
            btnText.transform.SetParent(go.transform, false);
            var tr = btnText.AddComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;

            var t = btnText.AddComponent<Text>();
            t.text = label;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.color = new Color(1f, 1f, 1f, 0.8f);

            var press = go.AddComponent<PointerPressHandler>();
            press.OnDown = down;
            press.OnUp = up;
        }

        private sealed class PointerPressHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
        {
            public System.Action OnDown;
            public System.Action OnUp;

            public void OnPointerDown(PointerEventData eventData) => OnDown?.Invoke();
            public void OnPointerUp(PointerEventData eventData) => OnUp?.Invoke();
            public void OnPointerExit(PointerEventData eventData) => OnUp?.Invoke();
        }
    }
}

