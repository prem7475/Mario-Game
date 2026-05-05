using MarioGame.Components.Audio;
using MarioGame.Components.FX;
using MarioGame.Components.Player;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Pickups
{
    public sealed class CoinPickup : MonoBehaviour
    {
        [SerializeField] private int value = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
                return;

            CoinManager.Instance?.AddCoins(value);
            AudioService.PlaySfx(SfxId.Coin);
            ParticleService.Burst(transform.position, new Color(1f, 0.85f, 0.15f), count: 12, size: 0.11f);
            Destroy(gameObject);
        }
    }
}
