using MarioGame.Components.Player;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Traps
{
    public sealed class TrapDamage : MonoBehaviour
    {
        [SerializeField] private int damage = 1;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
                return;

            var health = other.GetComponent<Health>();
            if (health == null)
                return;

            health.Damage(damage);
            if (health.Lives <= 0)
            {
                MarioGame.Levels.LevelRuntime.Current?.GameOver();
            }
            else
            {
                MarioGame.Levels.LevelRuntime.Current?.RespawnAtCheckpoint();
            }
        }
    }
}
