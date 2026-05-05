using MarioGame.Levels;
using MarioGame.Utils.Runtime;
using UnityEngine;

namespace MarioGame.Components.FX
{
    public sealed class ParallaxBackground : MonoBehaviour
    {
        private Transform _camera;

        public void Build(WorldThemeId theme)
        {
            _camera = UnityEngine.Camera.main != null ? UnityEngine.Camera.main.transform : null;
            if (_camera == null)
                return;

            Clear();

            var db = Resources.Load<VisualThemeDatabase>("Visuals/VisualThemeDatabase");
            var world = db != null ? db.Get(theme) : null;
            var parallax = world != null ? world.parallax : null;

            if (parallax == null)
            {
                CreateLayer("Far", sprite: null, new Color(1f, 1f, 1f, 0.15f), parallax: 0.15f, y: 2.0f, height: 5.2f);
                CreateLayer("Mid", sprite: null, new Color(1f, 1f, 1f, 0.22f), parallax: 0.28f, y: 1.2f, height: 4.0f);

                var tint = theme switch
                {
                    WorldThemeId.Jungle => new Color(0.15f, 0.55f, 0.2f, 0.28f),
                    WorldThemeId.Desert => new Color(0.75f, 0.55f, 0.2f, 0.28f),
                    WorldThemeId.Ice => new Color(0.75f, 0.85f, 1f, 0.25f),
                    WorldThemeId.Lava => new Color(0.85f, 0.25f, 0.2f, 0.25f),
                    WorldThemeId.Underwater => new Color(0.2f, 0.45f, 0.85f, 0.25f),
                    _ => new Color(1f, 1f, 1f, 0.2f)
                };

                CreateLayer("Near", sprite: null, tint, parallax: 0.42f, y: 0.0f, height: 3.1f);
                return;
            }

            CreateLayer("Far", parallax.farSprite, parallax.farTint, parallax.farParallax, parallax.farY, parallax.farHeight, parallax.tileWidth);
            CreateLayer("Mid", parallax.midSprite, parallax.midTint, parallax.midParallax, parallax.midY, parallax.midHeight, parallax.tileWidth);
            CreateLayer("Near", parallax.nearSprite, parallax.nearTint, parallax.nearParallax, parallax.nearY, parallax.nearHeight, parallax.tileWidth);
        }

        private void Clear()
        {
            for (var i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
        }

        private void CreateLayer(string name, Sprite sprite, Color tint, float parallax, float y, float height, float tileWidth = 20f)
        {
            var layer = new GameObject(name);
            layer.transform.SetParent(transform, false);
            layer.transform.position = new Vector3(0f, y, 10f);
            layer.AddComponent<ParallaxLayer>().Init(_camera, parallax);

            for (var i = 0; i < 3; i++)
            {
                var part = new GameObject($"Tile_{i}");
                part.transform.SetParent(layer.transform, false);
                part.transform.position = new Vector3(i * tileWidth, 0f, 10f);
                var sr = part.AddComponent<SpriteRenderer>();
                sr.sprite = sprite != null ? sprite : RuntimeSprites.Square;
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.size = new Vector2(tileWidth, height);
                sr.color = tint;
                sr.sortingOrder = -100;
            }
        }

        private sealed class ParallaxLayer : MonoBehaviour
        {
            private Transform _cam;
            private float _parallax;
            private Vector3 _startCamPos;
            private Vector3 _startPos;

            public void Init(Transform cam, float parallax)
            {
                _cam = cam;
                _parallax = Mathf.Clamp01(parallax);
                _startCamPos = _cam.position;
                _startPos = transform.position;
            }

            private void LateUpdate()
            {
                if (_cam == null)
                    return;

                var delta = _cam.position - _startCamPos;
                transform.position = _startPos + new Vector3(delta.x * _parallax, delta.y * _parallax, 0f);
            }
        }
    }
}
