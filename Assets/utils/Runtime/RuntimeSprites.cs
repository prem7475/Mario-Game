using UnityEngine;

namespace MarioGame.Utils.Runtime
{
    public static class RuntimeSprites
    {
        private static Sprite _square;
        private static Sprite _circle;

        public static Sprite Square
        {
            get
            {
                if (_square == null)
                    _square = CreateSquareSprite();
                return _square;
            }
        }

        public static Sprite Circle
        {
            get
            {
                if (_circle == null)
                    _circle = CreateCircleSprite();
                return _circle;
            }
        }

        private static Sprite CreateSquareSprite()
        {
            var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            tex.SetPixels(new[]
            {
                Color.white, Color.white,
                Color.white, Color.white
            });
            tex.Apply();
            tex.filterMode = FilterMode.Point;
            tex.wrapMode = TextureWrapMode.Repeat;
            return Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 32f);
        }

        private static Sprite CreateCircleSprite()
        {
            const int size = 32;
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            var center = new Vector2((size - 1) / 2f, (size - 1) / 2f);
            var radius = (size - 2) / 2f;

            for (var y = 0; y < size; y++)
            {
                for (var x = 0; x < size; x++)
                {
                    var d = Vector2.Distance(new Vector2(x, y), center);
                    var a = d <= radius ? 1f : 0f;
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }

            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            tex.wrapMode = TextureWrapMode.Clamp;
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 32f);
        }
    }
}

