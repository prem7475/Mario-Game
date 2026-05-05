using UnityEngine;

namespace MarioGame.Utils.Pooling
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class PooledRigidbody2DReset : MonoBehaviour, IPoolable
    {
        private Rigidbody2D _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        public void OnSpawned()
        {
            if (_rb == null) return;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }

        public void OnDespawned()
        {
            if (_rb == null) return;
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
        }
    }
}
