using UnityEngine;

namespace MarioGame.Components.Gameplay.Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public sealed class WalkerEnemy : MonoBehaviour
    {
        private float _leftX;
        private float _rightX;
        private float _speed;
        private Rigidbody2D _rb;
        private int _dir = 1;

        public void Configure(float leftX, float rightX, float speed)
        {
            _leftX = leftX;
            _rightX = rightX;
            _speed = Mathf.Max(0.2f, speed);
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            var x = transform.position.x;
            if (x <= _leftX) _dir = 1;
            if (x >= _rightX) _dir = -1;

            var v = _rb.linearVelocity;
            v.x = _dir * _speed;
            _rb.linearVelocity = v;
        }
    }
}
