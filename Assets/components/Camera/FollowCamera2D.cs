using UnityEngine;

namespace MarioGame.Components.Camera
{
    public sealed class FollowCamera2D : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 offset = new Vector2(2.5f, 1.1f);
        [SerializeField] private float smoothTime = 0.12f;
        [SerializeField] private float boundsPaddingX = 0.6f;
        [SerializeField] private float boundsPaddingY = 0.6f;

        private Vector3 _velocity;
        private MarioGame.Levels.LevelBounds _bounds;

        public void SetTarget(Transform t) => target = t;

        private void LateUpdate()
        {
            if (target == null)
                return;

            var desired = new Vector3(target.position.x + offset.x, target.position.y + offset.y, transform.position.z);
            var smoothed = Vector3.SmoothDamp(transform.position, desired, ref _velocity, smoothTime);

            if (_bounds == null)
                _bounds = Object.FindObjectOfType<MarioGame.Levels.LevelBounds>();

            if (_bounds != null)
            {
                var b = _bounds;
                var clamped = smoothed;
                clamped.x = Mathf.Clamp(clamped.x, b.minX + boundsPaddingX, b.maxX - boundsPaddingX);
                clamped.y = Mathf.Clamp(clamped.y, b.minY + boundsPaddingY, b.maxY - boundsPaddingY);
                smoothed = clamped;
            }

            transform.position = smoothed;
        }
    }
}
