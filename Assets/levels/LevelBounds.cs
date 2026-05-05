using UnityEngine;

namespace MarioGame.Levels
{
    public sealed class LevelBounds : MonoBehaviour
    {
        public float minX = -2f;
        public float maxX = 200f;
        public float minY = -2f;
        public float maxY = 40f;

        public Vector3 Clamp(Vector3 worldPosition)
        {
            worldPosition.x = Mathf.Clamp(worldPosition.x, minX, maxX);
            worldPosition.y = Mathf.Clamp(worldPosition.y, minY, maxY);
            return worldPosition;
        }
    }
}

