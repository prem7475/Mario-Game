using UnityEngine;

namespace MarioGame.Components.FX
{
    public sealed class HitFlash : MonoBehaviour
    {
        [SerializeField] private Color flashColor = Color.white;
        [SerializeField] private float duration = 0.08f;

        private SpriteRenderer _sr;
        private Color _base;
        private float _t;
        private bool _active;

        private void Awake()
        {
            _sr = GetComponent<SpriteRenderer>();
            if (_sr != null)
                _base = _sr.color;
        }

        public void Flash()
        {
            if (_sr == null)
                return;

            _active = true;
            _t = duration;
            _sr.color = flashColor;
        }

        private void Update()
        {
            if (!_active)
                return;

            _t -= Time.deltaTime;
            if (_t <= 0f)
            {
                _active = false;
                _sr.color = _base;
            }
        }
    }
}

