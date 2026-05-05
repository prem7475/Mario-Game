using MarioGame.Components.FX;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Traps
{
    public sealed class FireTrap : MonoBehaviour
    {
        [SerializeField] private float onSeconds = 1.2f;
        [SerializeField] private float offSeconds = 1.0f;
        [SerializeField] private int damage = 1;

        private bool _on = true;
        private float _t;
        private Collider2D _col;
        private SpriteRenderer _sr;

        private void Awake()
        {
            _col = GetComponent<Collider2D>();
            _sr = GetComponent<SpriteRenderer>();
            SetState(true);
        }

        private void Update()
        {
            _t += Time.deltaTime;
            var limit = _on ? onSeconds : offSeconds;
            if (_t >= limit)
            {
                _t = 0f;
                SetState(!_on);
            }
        }

        private void SetState(bool on)
        {
            _on = on;
            if (_col != null) _col.enabled = on;
            if (_sr != null) _sr.color = on ? new Color(1f, 0.45f, 0.15f, 0.25f) : new Color(1f, 0.45f, 0.15f, 0.05f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!_on)
                return;

            var health = other.GetComponentInParent<MarioGame.Components.Gameplay.Health>();
            if (health == null)
                return;

            if (other.GetComponentInParent<MarioGame.Components.Player.PlayerMarker>() == null)
                return;

            health.Damage(damage);
            ParticleService.Burst(other.transform.position, new Color(1f, 0.45f, 0.15f), count: 10, size: 0.12f);
            if (health.Lives <= 0)
                MarioGame.Levels.LevelRuntime.Current?.GameOver();
            else
                MarioGame.Levels.LevelRuntime.Current?.RespawnAtCheckpoint();
        }
    }
}

