using MarioGame.Components.Player;
using MarioGame.Levels;
using MarioGame.Utils.Runtime;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class BossController : MonoBehaviour
    {
        [SerializeField] private float moveAmplitude = 2.5f;
        [SerializeField] private float moveSpeed = 1.4f;
        [SerializeField] private float fireInterval = 1.25f;

        private float _originX;
        private float _timeSinceFire;
        private Rigidbody2D _rb;

        public void Configure(int levelNumber)
        {
            // Slight scaling per boss tier
            var tier = Mathf.Max(1, levelNumber / 10);
            moveSpeed += tier * 0.05f;
            fireInterval = Mathf.Max(0.65f, fireInterval - tier * 0.04f);
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _originX = transform.position.x;
        }

        private void FixedUpdate()
        {
            var targetX = _originX + Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
            var p = _rb.position;
            p.x = Mathf.Lerp(p.x, targetX, 0.12f);
            _rb.MovePosition(p);
        }

        private void Update()
        {
            _timeSinceFire += Time.deltaTime;
            if (_timeSinceFire < fireInterval)
                return;

            _timeSinceFire = 0f;
            Fire();
        }

        private void Fire()
        {
            if (LevelRuntime.Current == null)
                return;

            var proj = new GameObject("BossFire");
            proj.transform.position = transform.position + new Vector3(-1.2f, 0.2f, 0f);

            var sr = proj.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Circle;
            sr.color = new Color(1f, 0.45f, 0.15f);
            proj.transform.localScale = new Vector3(0.55f, 0.55f, 1f);

            var rb = proj.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.velocity = new Vector2(-7.5f, Mathf.Sin(Time.time * 2f) * 1.1f);

            var col = proj.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.35f;

            proj.AddComponent<BossFireProjectile>();
            Destroy(proj, 6f);
        }
    }

    public sealed class BossFireProjectile : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
                return;

            var health = other.GetComponentInParent<MarioGame.Components.Gameplay.Health>();
            if (health != null)
            {
                health.Damage(1);
                if (health.Lives <= 0)
                    LevelRuntime.Current?.GameOver();
                else
                    LevelRuntime.Current?.RespawnAtCheckpoint();
            }

            Destroy(gameObject);
        }
    }
}
