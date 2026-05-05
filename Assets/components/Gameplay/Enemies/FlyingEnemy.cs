using UnityEngine;

namespace MarioGame.Components.Gameplay.Enemies
{
    public sealed class FlyingEnemy : MonoBehaviour
    {
        private float _amplitude = 0.6f;
        private float _frequency = 1.6f;
        private float _driftSpeed = 0.8f;
        private float _baseY;

        public void Configure(float amplitude, float frequency, float driftSpeed)
        {
            _amplitude = Mathf.Max(0.05f, amplitude);
            _frequency = Mathf.Max(0.05f, frequency);
            _driftSpeed = Mathf.Max(0f, driftSpeed);
        }

        private void Start()
        {
            _baseY = transform.position.y;
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.velocity = Vector2.zero;
            }
        }

        private void Update()
        {
            var p = transform.position;
            p.y = _baseY + Mathf.Sin(Time.time * _frequency) * _amplitude;
            p.x += _driftSpeed * Time.deltaTime;
            transform.position = p;
        }
    }
}

