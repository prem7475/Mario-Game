using MarioGame.Components.Player;
using MarioGame.Levels;
using UnityEngine;

namespace MarioGame.Components.Gameplay
{
    public sealed class GoalFlag : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<PlayerMarker>() == null)
                return;

            LevelRuntime.Current?.CompleteLevel();
        }
    }
}

