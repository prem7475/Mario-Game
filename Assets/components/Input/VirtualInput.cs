namespace MarioGame.Components.Input
{
    public static class VirtualInput
    {
        public static float Horizontal;
        public static bool JumpHeld;

        private static bool _jumpPressed;

        public static void PressLeft(bool pressed)
        {
            Horizontal = pressed ? -1f : (Horizontal < 0 ? 0 : Horizontal);
        }

        public static void PressRight(bool pressed)
        {
            Horizontal = pressed ? 1f : (Horizontal > 0 ? 0 : Horizontal);
        }

        public static void PressJump(bool pressed)
        {
            JumpHeld = pressed;
            if (pressed)
                _jumpPressed = true;
        }

        public static bool ConsumeJumpPressed()
        {
            var v = _jumpPressed;
            _jumpPressed = false;
            return v;
        }
    }
}

