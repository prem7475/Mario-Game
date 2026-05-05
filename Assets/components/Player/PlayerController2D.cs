using MarioGame.Components.Audio;
using MarioGame.Components.Input;
using MarioGame.Physics;
using UnityEngine;

namespace MarioGame.Components.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlayerController2D : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;
        [SerializeField] private float acceleration = 60f;
        [SerializeField] private float airAcceleration = 40f;
        [SerializeField] private float maxFallSpeed = 22f;

        [Header("Jump")]
        [SerializeField] private float jumpVelocity = 14f;
        [SerializeField] private float doubleJumpVelocity = 13f;
        [SerializeField] private float coyoteTime = 0.09f;
        [SerializeField] private float jumpBuffer = 0.12f;
        [SerializeField] private float jumpCutMultiplier = 0.55f;
        [SerializeField] private bool enableDoubleJump = true;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.55f);
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.2f);

        private Rigidbody2D _rb;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private bool _grounded;
        private bool _usedDoubleJump;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.gravityScale = 3.4f;
        }

        private void Update()
        {
            var input = InputService.Instance;
            if (input == null)
                return;

            _jumpBufferTimer -= Time.deltaTime;
            if (input.JumpPressedThisFrame)
                _jumpBufferTimer = jumpBuffer;

            _grounded = GroundCheck.IsGrounded(transform.position, groundCheckOffset, groundCheckSize, groundMask);
            if (_grounded)
            {
                _coyoteTimer = coyoteTime;
                _usedDoubleJump = false;
            }
            else
            {
                _coyoteTimer -= Time.deltaTime;
            }

            if (!input.JumpHeld && _rb.velocity.y > 0.1f)
                _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * jumpCutMultiplier);
        }

        private void FixedUpdate()
        {
            var input = InputService.Instance;
            if (input == null)
                return;

            var targetX = input.Horizontal * moveSpeed;
            var accel = _grounded ? acceleration : airAcceleration;
            var newX = Mathf.MoveTowards(_rb.velocity.x, targetX, accel * Time.fixedDeltaTime);

            var newY = Mathf.Max(_rb.velocity.y, -maxFallSpeed);
            _rb.velocity = new Vector2(newX, newY);

            TryConsumeJump(input);
        }

        private void TryConsumeJump(InputService input)
        {
            if (_jumpBufferTimer <= 0f)
                return;

            if (_coyoteTimer > 0f)
            {
                _jumpBufferTimer = 0f;
                _coyoteTimer = 0f;
                _rb.velocity = new Vector2(_rb.velocity.x, jumpVelocity);
                AudioService.PlaySfx(SfxId.Jump);
                return;
            }

            if (enableDoubleJump && !_usedDoubleJump && !_grounded)
            {
                _jumpBufferTimer = 0f;
                _usedDoubleJump = true;
                _rb.velocity = new Vector2(_rb.velocity.x, doubleJumpVelocity);
                AudioService.PlaySfx(SfxId.Jump);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = _grounded ? Color.green : Color.yellow;
            var center = (Vector2)transform.position + groundCheckOffset;
            Gizmos.DrawWireCube(center, groundCheckSize);
        }
    }
}

