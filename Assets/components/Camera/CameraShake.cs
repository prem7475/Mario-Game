using UnityEngine;

namespace MarioGame.Components.Camera
{
    public sealed class CameraShake : MonoBehaviour
    {
        [SerializeField] private float traumaDecay = 1.8f;
        [SerializeField] private float maxTranslation = 0.25f;
        [SerializeField] private float maxRotation = 1.1f;
        [SerializeField] private float frequency = 25f;

        private float _trauma;
        private Vector3 _baseLocalPos;
        private Quaternion _baseLocalRot;

        private void Awake()
        {
            _baseLocalPos = transform.localPosition;
            _baseLocalRot = transform.localRotation;
        }

        private void LateUpdate()
        {
            if (_trauma <= 0f)
            {
                transform.localPosition = _baseLocalPos;
                transform.localRotation = _baseLocalRot;
                return;
            }

            _trauma = Mathf.Max(0f, _trauma - traumaDecay * Time.deltaTime);
            var shake = _trauma * _trauma;

            var t = Time.time * frequency;
            var offset = new Vector3(
                (Mathf.PerlinNoise(t, 0.1f) - 0.5f) * 2f,
                (Mathf.PerlinNoise(0.2f, t) - 0.5f) * 2f,
                0f) * (maxTranslation * shake);

            var rotZ = (Mathf.PerlinNoise(t, t) - 0.5f) * 2f * (maxRotation * shake);
            transform.localPosition = _baseLocalPos + offset;
            transform.localRotation = _baseLocalRot * Quaternion.Euler(0f, 0f, rotZ);
        }

        public void AddTrauma(float amount)
        {
            _trauma = Mathf.Clamp01(_trauma + Mathf.Abs(amount));
        }
    }
}

