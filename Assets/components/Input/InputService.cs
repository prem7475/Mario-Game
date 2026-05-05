using UnityEngine;

namespace MarioGame.Components.Input
{
    public sealed class InputService : MonoBehaviour
    {
        public static InputService Instance { get; private set; }

        public float Horizontal { get; private set; }
        public bool JumpPressedThisFrame { get; private set; }
        public bool JumpHeld { get; private set; }

        private bool _jumpPressedLatched;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            JumpPressedThisFrame = false;

            var keyboardAxis = UnityEngine.Input.GetAxisRaw("Horizontal");
            var keyboardJumpDown = UnityEngine.Input.GetButtonDown("Jump");
            var keyboardJumpHeld = UnityEngine.Input.GetButton("Jump");

            Horizontal = Mathf.Clamp(VirtualInput.Horizontal + keyboardAxis, -1f, 1f);

            JumpHeld = VirtualInput.JumpHeld || keyboardJumpHeld;

            if (VirtualInput.ConsumeJumpPressed() || keyboardJumpDown || _jumpPressedLatched)
            {
                JumpPressedThisFrame = true;
                _jumpPressedLatched = false;
            }
        }

        public void LatchJumpPressed()
        {
            _jumpPressedLatched = true;
        }
    }
}

