using MarioGame.Components.Audio;
using MarioGame.Components.FX;
using MarioGame.Components.Player;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Pickups
{
    public sealed class StrawberryPickup : MonoBehaviour
    {
        [SerializeField] private int extraLives = 1;
        [SerializeField] private float durationSeconds = 10f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
                return;

            var power = other.GetComponent<PlayerPowerState>();
            if (power != null)
                power.ApplyStrawberryPowerup(durationSeconds);

            var health = other.GetComponent<Health>();
            if (health != null)
                health.AddLife(extraLives);

            AudioService.PlaySfx(SfxId.Powerup);
            ParticleService.Burst(transform.position, new Color(1f, 0.2f, 0.6f), count: 18, size: 0.14f);
            Destroy(gameObject);
        }
    }
}
