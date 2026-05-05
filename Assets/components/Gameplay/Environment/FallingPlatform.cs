using MarioGame.Components.Player;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class FallingPlatform : MonoBehaviour
    {
        [SerializeField] public float delaySeconds = 0.25f;
        [SerializeField] public float respawnSeconds = 999f;

        private Rigidbody2D _rb;
        private Vector3 _startPos;
        private bool _triggered;
        private float _timer;
        private float _respawnTimer;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb == null) _rb = gameObject.AddComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.gravityScale = 3.4f;
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _startPos = transform.position;
        }

        private void Update()
        {
            if (_triggered && _rb.bodyType == RigidbodyType2D.Kinematic)
            {
                _timer += Time.unscaledDeltaTime;
                if (_timer >= delaySeconds)
                    _rb.bodyType = RigidbodyType2D.Dynamic;
            }

            if (_rb.bodyType == RigidbodyType2D.Dynamic && respawnSeconds < 998f)
            {
                _respawnTimer += Time.unscaledDeltaTime;
                if (_respawnTimer >= respawnSeconds)
                    Respawn();
            }
        }

        private void Respawn()
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.velocity = Vector2.zero;
            transform.position = _startPos;
            _triggered = false;
            _timer = 0f;
            _respawnTimer = 0f;
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (_triggered)
                return;

            if (collision.collider.GetComponentInParent<PlayerMarker>() != null)
            {
                _triggered = true;
                _timer = 0f;
                _respawnTimer = 0f;
            }
        }
    }
}

