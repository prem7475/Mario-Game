using UnityEngine;

namespace MarioGame.Components.Gameplay
{
    public sealed class PlayerPowerState : MonoBehaviour
    {
        public bool IsBig { get; private set; }

        [SerializeField] private Vector3 bigScale = new Vector3(1.35f, 1.35f, 1f);
        [SerializeField] private Vector3 smallScale = Vector3.one;

        private Coroutine _timer;

        public void ApplyStrawberryPowerup(float durationSeconds = 10f)
        {
            IsBig = true;
            transform.localScale = bigScale;

            if (_timer != null)
                StopCoroutine(_timer);

            if (durationSeconds > 0.01f)
                _timer = StartCoroutine(PowerTimer(durationSeconds));
        }

        public void ResetPower()
        {
            IsBig = false;
            transform.localScale = smallScale;
            _timer = null;
        }

        private System.Collections.IEnumerator PowerTimer(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            ResetPower();
        }
    }
}
