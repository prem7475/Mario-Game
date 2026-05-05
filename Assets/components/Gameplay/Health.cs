using UnityEngine;

namespace MarioGame.Components.Gameplay
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField] private int maxLives = 3;

        public int Lives { get; private set; }

        private void Awake()
        {
            Lives = maxLives;
        }

        public void AddLife(int amount)
        {
            Lives = Mathf.Clamp(Lives + amount, 0, 99);
        }

        public void Damage(int amount)
        {
            Lives = Mathf.Max(0, Lives - amount);
        }
    }
}

