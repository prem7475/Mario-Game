using MarioGame.Components.Gameplay;
using MarioGame.Components.Player;
using UnityEngine;

namespace MarioGame.Components.Gameplay.Environment
{
    [RequireComponent(typeof(Collider2D))]
    public sealed class BreakableBlock : MonoBehaviour
    {
        [SerializeField] private float requiredUpwardSpeed = 6f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            var player = collision.collider.GetComponentInParent<PlayerMarker>();
            if (player == null)
                return;

            var power = player.GetComponent<PlayerPowerState>();
            if (power == null || !power.IsBig)
                return;

            // Break if player hits from below with enough upward velocity
            foreach (var c in collision.contacts)
            {
                if (c.normal.y < -0.6f)
                {
                    var rb = player.GetComponent<Rigidbody2D>();
                    if (rb != null && rb.linearVelocity.y >= requiredUpwardSpeed)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }
            }
        }
    }
}
