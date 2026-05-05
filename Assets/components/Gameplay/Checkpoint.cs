using MarioGame.Components.Player;
using MarioGame.Levels;
using UnityEngine;

namespace MarioGame.Components.Gameplay
{
    public sealed class Checkpoint : MonoBehaviour
    {
        private bool _active;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
                return;

            if (_active)
                return;

            _active = true;
            LevelRuntime.Current?.SetCheckpoint(transform.position + Vector3.up * 1.2f);
            var sr = GetComponent<SpriteRenderer>();
            if (sr != null)
                sr.color = new Color(0.2f, 1f, 0.2f);
        }
    }
}

