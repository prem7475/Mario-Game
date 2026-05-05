using UnityEngine;

namespace MarioGame.Components.FX
{
    public sealed class GlowPulse : MonoBehaviour
    {
        [SerializeField] private float scalePulse = 0.08f;
        [SerializeField] private float speed = 2.8f;
        [SerializeField] private float alphaPulse = 0.12f;

        private Vector3 _baseScale;
        private SpriteRenderer _sr;
        private Color _baseColor;

        private void Awake()
        {
            _baseScale = transform.localScale;
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null)
                _baseColor = _sr.color;
        }

        private void Update()
        {
            var s = 1f + Mathf.Sin(Time.time * speed) * scalePulse;
            transform.localScale = _baseScale * s;

            if (_sr != null)
            {
                var a = _baseColor.a + Mathf.Sin(Time.time * speed * 1.15f) * alphaPulse;
                var c = _baseColor;
                c.a = Mathf.Clamp01(a);
                _sr.color = c;
            }
        }
    }
}

