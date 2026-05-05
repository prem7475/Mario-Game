using MarioGame.Components.Audio;
using MarioGame.Components.Camera;
using UnityEngine;

namespace MarioGame.Components.Gameplay
{
    [RequireComponent(typeof(Health))]
    public sealed class PlayerDamageReceiver : MonoBehaviour
    {
        [SerializeField] private float invulnSeconds = 1.0f;
        [SerializeField] private float blinkHz = 12f;
        [SerializeField] private Vector2 knockback = new Vector2(-4.5f, 6.5f);

        private Health _health;
        private float _invulnTimer;
        private SpriteRenderer[] _renderers;
        private Rigidbody2D _rb;
        private FollowCamera2D _camera;

        public bool IsInvulnerable => _invulnTimer > 0f;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _renderers = GetComponentsInChildren<SpriteRenderer>(true);
            _rb = GetComponent<Rigidbody2D>();
            _camera = UnityEngine.Camera.main != null ? UnityEngine.Camera.main.GetComponent<FollowCamera2D>() : null;
        }

        private void Update()
        {
            if (_invulnTimer <= 0f)
                return;

            _invulnTimer -= Time.deltaTime;
            var on = Mathf.Sin(Time.time * blinkHz * Mathf.PI * 2f) > 0f;
            for (var i = 0; i < _renderers.Length; i++)
                if (_renderers[i] != null) _renderers[i].enabled = on;

            if (_invulnTimer <= 0f)
            {
                for (var i = 0; i < _renderers.Length; i++)
                    if (_renderers[i] != null) _renderers[i].enabled = true;
            }
        }

        public void ApplyDamage(int amount, Vector2 sourcePosition)
        {
            if (amount <= 0 || IsInvulnerable)
                return;

            _health.Damage(amount);
            _invulnTimer = invulnSeconds;

            if (_rb != null)
            {
                var dir = Mathf.Sign(transform.position.x - sourcePosition.x);
                if (dir == 0) dir = 1f;
                _rb.linearVelocity = new Vector2(knockback.x * dir, knockback.y);
            }

            _camera?.Shake(0.22f);
            AudioService.PlaySfx(SfxId.EnemyHit);
        }
    }
}
