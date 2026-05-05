using MarioGame.Components.Player;
using MarioGame.Components.FX;
using UnityEngine;
using MarioGame.Components.Gameplay;

namespace MarioGame.Components.Gameplay.Enemies
{
    public sealed class EnemyDamageOnTouch : MonoBehaviour
    {
        [SerializeField] private int damage = 1;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.GetComponentInParent<PlayerMarker>() == null)
                return;

            // Stomp detection: player hits enemy from above -> destroy enemy and bounce player.
            foreach (var c in collision.contacts)
            {
                // Normal points from enemy to other collider. If player is above, normal is upwards.
                    if (c.normal.y > 0.55f)
                    {
                    var prb = collision.collider.attachedRigidbody;
                    if (prb != null && prb.linearVelocity.y <= 0.5f)
                    {
                        prb.linearVelocity = new Vector2(prb.linearVelocity.x, 10.5f);
                        ParticleService.Burst(transform.position, new Color(0.75f, 0.35f, 0.15f), count: 10, size: 0.12f);
                        Destroy(gameObject);
                        return;
                    }
                    }
            }

            var health = collision.collider.GetComponent<Health>();
            if (health == null)
                return;

            var receiver = collision.collider.GetComponent<PlayerDamageReceiver>();
            if (receiver != null)
                receiver.ApplyDamage(damage, transform.position);
            else
                health.Damage(damage);
            if (health.Lives <= 0)
                MarioGame.Levels.LevelRuntime.Current?.GameOver();
            else
                MarioGame.Levels.LevelRuntime.Current?.RespawnAtCheckpoint();
        }
    }
}
