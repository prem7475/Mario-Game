using MarioGame.Components.FX;
using MarioGame.Components.Gameplay;
using MarioGame.Components.Player;
using MarioGame.Levels;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class EnemyAI : MonoBehaviour
    {
        [Header("Patrol")]
        [SerializeField] private float leftX;
        [SerializeField] private float rightX;
        [SerializeField] private float speed = 1.6f;

        [Header("Combat")]
        [SerializeField] private int touchDamage = 1;
        [SerializeField] private float stompBounceVelocity = 10.5f;

        private Rigidbody2D _rb;
        private int _dir = 1;

        public void ConfigurePatrol(float left, float right, float patrolSpeed)
        {
            leftX = Mathf.Min(left, right);
            rightX = Mathf.Max(left, right);
            speed = Mathf.Max(0.2f, patrolSpeed);
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        private void FixedUpdate()
        {
            var x = transform.position.x;
            if (x <= leftX) _dir = 1;
            if (x >= rightX) _dir = -1;

            var v = _rb.velocity;
            v.x = _dir * speed;
            _rb.velocity = v;

            var s = transform.localScale;
            s.x = Mathf.Abs(s.x) * (_dir >= 0 ? 1 : -1);
            transform.localScale = s;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.collider.GetComponentInParent<PlayerMarker>();
            if (player == null)
                return;

            // Stomp: player hits enemy from above -> destroy enemy and bounce player.
            foreach (var c in collision.contacts)
            {
                if (c.normal.y > 0.55f)
                {
                    var prb = player.GetComponent<Rigidbody2D>();
                    if (prb != null && prb.velocity.y <= 0.5f)
                    {
                        prb.velocity = new Vector2(prb.velocity.x, stompBounceVelocity);
                        ParticleService.Burst(transform.position, new Color(0.75f, 0.35f, 0.15f), count: 10, size: 0.12f);
                        Destroy(gameObject);
                        return;
                    }
                }
            }

            var health = player.GetComponent<Health>();
            if (health == null)
                return;

            health.Damage(touchDamage);
            if (health.Lives <= 0)
                LevelRuntime.Current?.GameOver();
            else
                LevelRuntime.Current?.RespawnAtCheckpoint();
        }
    }
}

