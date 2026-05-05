using UnityEngine;

namespace MarioGame.Physics
{
    public static class GroundCheck
    {
        public static bool IsGrounded(
            Vector2 worldPosition,
            Vector2 offset,
            Vector2 size,
            LayerMask groundMask)
        {
            var center = worldPosition + offset;
            var hit = Physics2D.OverlapBox(center, size, 0f, groundMask);
            return hit != null;
        }
    }
}

