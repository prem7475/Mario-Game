using System;
using MarioGame.Levels;
using MarioGame.Utils.Save;
using UnityEngine;
using MarioGame.Utils.Analytics;

namespace MarioGame.Components.Monetization.IAP
{
    public sealed class IapManager : MonoBehaviour
    {
        public static IapManager Instance { get; private set; }

        public bool IsInitialized { get; private set; }

        public event Action<string> OnPurchaseSucceeded;
        public event Action<string, string> OnPurchaseFailed;

        private SaveService _save;
        private LevelProgression _progression;

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

        public void Construct(SaveService saveService, LevelProgression progression)
        {
            _save = saveService;
            _progression = progression;
            Initialize();
        }

        private void Initialize()
        {
#if UNITY_PURCHASING
            var impl = gameObject.AddComponent<UnityIapImpl>();
            impl.Construct(this);
            impl.InitializePurchasing();
#else
            Debug.LogWarning("Unity IAP package not installed. IAP disabled. Install 'In-App Purchasing' package (Unity) to enable.");
            IsInitialized = false;
#endif
        }

        public void Purchase(string productId)
        {
#if UNITY_PURCHASING
            var impl = GetComponent<UnityIapImpl>();
            if (impl != null) impl.Purchase(productId);
            else OnPurchaseFailed?.Invoke(productId, "IAP not ready");
#else
            OnPurchaseFailed?.Invoke(productId, "IAP package missing");
#endif
        }

        internal void MarkInitialized(bool initialized)
        {
            IsInitialized = initialized;
        }

        internal void GrantEntitlement(string productId)
        {
            if (_save == null || _progression == null)
                return;

            if (productId == ProductIds.UnlockAllLevels)
            {
                _save.Data.unlockedLevel = 100;
                _save.Data.purchasedUnlockAllLevels = true;
                _save.Save();
            }
            else if (productId == ProductIds.ExtraLivesSmall)
            {
                _save.Data.inventoryExtraLives += 5;
                _save.Save();
            }
            else if (productId == ProductIds.CoinPackSmall)
            {
                _save.Data.softCoins += 500;
                _save.Save();
            }

            OnPurchaseSucceeded?.Invoke(productId);
            LocalAnalytics.Track("iap_purchase_" + productId);
        }

        internal void FailPurchase(string productId, string reason)
        {
            OnPurchaseFailed?.Invoke(productId, reason);
            LocalAnalytics.Track("iap_failed_" + productId);
        }
    }
}
