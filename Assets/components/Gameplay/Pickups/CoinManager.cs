using UnityEngine;

namespace MarioGame.Components.Gameplay.Pickups
{
    public sealed class CoinManager : MonoBehaviour
    {
        public static CoinManager Instance { get; private set; }

        public int Coins { get; private set; }
        public int TotalCoins { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void ResetForLevel()
        {
            Coins = 0;
            TotalCoins = 0;
        }

        public void RegisterCoinSpawned(int amount = 1)
        {
            TotalCoins += Mathf.Max(0, amount);
        }

        public void AddCoins(int amount)
        {
            Coins += Mathf.Max(0, amount);
        }
    }
}

