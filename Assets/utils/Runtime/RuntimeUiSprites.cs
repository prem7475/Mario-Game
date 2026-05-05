using UnityEngine;

namespace MarioGame.Utils.Runtime
{
    public static class RuntimeUiSprites
    {
        private static Sprite _rounded;

        public static Sprite RoundedRect
        {
            get
            {
                if (_rounded == null)
                    _rounded = CreateRoundedRectSprite(64, 22);
                return _rounded;
            }
        }

        private static Sprite CreateRoundedRectSprite(int size, int radius)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            var r = Mathf.Clamp(radius, 1, size / 2 - 1);
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                // Signed distance to rounded rectangle centered at (0,0) in [-1,1] space.
                var p = new Vector2((x + 0.5f) / size * 2f - 1f, (y + 0.5f) / size * 2f - 1f);
                var half = new Vector2(0.92f, 0.92f);
                var rr = (float)r / (size / 2f);
                var q = new Vector2(Mathf.Abs(p.x), Mathf.Abs(p.y)) - (half - new Vector2(rr, rr));
                var outside = new Vector2(Mathf.Max(q.x, 0f), Mathf.Max(q.y, 0f)).magnitude;
                var inside = Mathf.Min(Mathf.Max(q.x, q.y), 0f);
                var dist = outside + inside - rr;

                // Smooth edge
                var a = Mathf.SmoothStep(1f, 0f, Mathf.InverseLerp(-0.03f, 0.03f, dist));
                tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
            }

            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 64f, 0, SpriteMeshType.FullRect, new Vector4(18, 18, 18, 18));
        }
    }
}

