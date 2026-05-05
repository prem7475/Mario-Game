using MarioGame.Components.Audio;
using MarioGame.Components.FX;
using MarioGame.Components.Input;
using MarioGame.Physics;
using UnityEngine;

namespace MarioGame.Components.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8.5f;
        [SerializeField] private float acceleration = 65f;
        [SerializeField] private float airAcceleration = 45f;
        [SerializeField] private float maxFallSpeed = 22f;

        [Header("Jump")]
        [SerializeField] private float jumpVelocity = 14.5f;
        [SerializeField] private float doubleJumpVelocity = 13.5f;
        [SerializeField] private float coyoteTime = 0.09f;
        [SerializeField] private float jumpBuffer = 0.12f;
        [SerializeField] private float jumpCutMultiplier = 0.55f;
        [SerializeField] private bool enableDoubleJump = true;
        [SerializeField] private float fallGravityMultiplier = 1.65f;
        [SerializeField] private float lowJumpGravityMultiplier = 1.35f;

        [Header("Ground Check")]
        [SerializeField] private LayerMask groundMask = ~0;
        [SerializeField] private Vector2 groundCheckOffset = new Vector2(0f, -0.72f);
        [SerializeField] private Vector2 groundCheckSize = new Vector2(0.65f, 0.18f);

        [Header("Animation")]
        [SerializeField] private Animator animator;
        [SerializeField] private string animParamSpeed = "Speed";
        [SerializeField] private string animParamGrounded = "Grounded";
        [SerializeField] private string animParamJump = "Jump";

        public bool IsGrounded { get; private set; }
        public Vector2 Velocity => _rb != null ? _rb.linearVelocity : Vector2.zero;

        private Rigidbody2D _rb;
        private float _coyoteTimer;
        private float _jumpBufferTimer;
        private bool _usedDoubleJump;
        private int _facing = 1;
        private JumpDust _dust;
        private bool _wasGrounded;
        private MarioGame.Components.Camera.FollowCamera2D _camera;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.gravityScale = 3.4f;

            if (animator == null)
                animator = GetComponentInChildren<Animator>();

            _dust = GetComponent<JumpDust>();
            if (_dust == null)
                _dust = gameObject.AddComponent<JumpDust>();

            _camera = UnityEngine.Camera.main != null ? UnityEngine.Camera.main.GetComponent<MarioGame.Components.Camera.FollowCamera2D>() : null;
        }

        private void Update()
        {
            var input = InputService.Instance;
            if (input == null)
                return;

            _jumpBufferTimer -= Time.deltaTime;
            if (input.JumpPressedThisFrame)
                _jumpBufferTimer = jumpBuffer;

            // Exclude the player's own layer from ground checks so it works out-of-the-box.
            var mask = groundMask & ~(1 << gameObject.layer);
            IsGrounded = GroundCheck.IsGrounded(transform.position, groundCheckOffset, groundCheckSize, mask);

            if (IsGrounded)
            {
                _coyoteTimer = coyoteTime;
                _usedDoubleJump = false;
            }
            else
            {
                _coyoteTimer -= Time.deltaTime;
            }

            // Variable jump height.
            if (!input.JumpHeld && _rb.linearVelocity.y > 0.1f)
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * jumpCutMultiplier);

            // Better jump arc: stronger gravity on fall + when releasing early.
            if (_rb.linearVelocity.y < -0.01f)
            {
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.deltaTime;
            }
            else if (_rb.linearVelocity.y > 0.01f && !input.JumpHeld)
            {
                _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpGravityMultiplier - 1f) * Time.deltaTime;
            }

            UpdateAnimator(input);

            if (!_wasGrounded && IsGrounded)
            {
                // landing puff
                _dust.Puff(transform.position + Vector3.down * 0.6f);
            }

            _wasGrounded = IsGrounded;
        }

        private void FixedUpdate()
        {
            var input = InputService.Instance;
            if (input == null)
                return;

            var targetX = input.Horizontal * moveSpeed;
            var accel = IsGrounded ? acceleration : airAcceleration;
            var newX = Mathf.MoveTowards(_rb.linearVelocity.x, targetX, accel * Time.fixedDeltaTime);

            var newY = Mathf.Max(_rb.linearVelocity.y, -maxFallSpeed);
            _rb.linearVelocity = new Vector2(newX, newY);

            if (Mathf.Abs(input.Horizontal) > 0.01f)
            {
                _facing = input.Horizontal > 0 ? 1 : -1;
                var s = transform.localScale;
                s.x = Mathf.Abs(s.x) * _facing;
                transform.localScale = s;
            }

            TryConsumeJump();
        }

        private void TryConsumeJump()
        {
            if (_jumpBufferTimer <= 0f)
                return;

            if (_coyoteTimer > 0f)
            {
                _jumpBufferTimer = 0f;
                _coyoteTimer = 0f;
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpVelocity);
                AudioService.PlaySfx(SfxId.Jump);
                _dust.Puff(transform.position + Vector3.down * 0.6f);
                _camera?.Shake(0.07f);
                TriggerJumpAnim();
                return;
            }

            if (enableDoubleJump && !_usedDoubleJump && !IsGrounded)
            {
                _jumpBufferTimer = 0f;
                _usedDoubleJump = true;
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, doubleJumpVelocity);
                AudioService.PlaySfx(SfxId.Jump);
                _dust.Puff(transform.position + Vector3.down * 0.6f);
                _camera?.Shake(0.05f);
                TriggerJumpAnim();
            }
        }

        private void UpdateAnimator(InputService input)
        {
            if (animator == null)
                return;

            animator.SetBool(animParamGrounded, IsGrounded);
            animator.SetFloat(animParamSpeed, Mathf.Abs(_rb.linearVelocity.x));

            if (!IsGrounded)
                animator.SetBool(animParamJump, true);
            else
                animator.SetBool(animParamJump, false);
        }

        private void TriggerJumpAnim()
        {
            if (animator == null)
                return;

            // Optional: if user adds a Trigger named "Jump" later, this won't error.
            // Animator.SetTrigger silently ignores missing parameters, but only in newer Unity.
            // So we keep it as a bool toggle via UpdateAnimator.
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = IsGrounded ? Color.green : Color.yellow;
            var center = (Vector2)transform.position + groundCheckOffset;
            Gizmos.DrawWireCube(center, groundCheckSize);
        }
    }
}
