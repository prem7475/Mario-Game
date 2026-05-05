using UnityEngine;

namespace MarioGame.Components.Gameplay.Environment
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class MovingPlatform : MonoBehaviour
    {
        [SerializeField] public Vector2 localOffset = new Vector2(2.5f, 0f);
        [SerializeField] public float periodSeconds = 2.6f;

        private Vector3 _start;
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Kinematic;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _start = transform.position;
        }

        private void FixedUpdate()
        {
            if (periodSeconds <= 0.1f)
                return;

            var t = (Mathf.Sin(Time.time * (2f * Mathf.PI / periodSeconds)) + 1f) * 0.5f;
            var target = _start + (Vector3)(localOffset * Mathf.Lerp(-1f, 1f, t));
            _rb.MovePosition(target);
        }
    }
}
