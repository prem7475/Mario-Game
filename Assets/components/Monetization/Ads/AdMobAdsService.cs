using System;
using UnityEngine;

namespace MarioGame.Components.Monetization.Ads
{
    // This compiles even if the Google Mobile Ads plugin is not installed.
    // After importing the AdMob Unity plugin, add Scripting Define Symbol: ADMOB_SDK
    public sealed class AdMobAdsService : IAdsService
    {
        public bool IsInitialized { get; private set; }
        public bool IsRewardedReady => IsInitialized && _rewardedReady;
        public bool IsInterstitialReady => IsInitialized && _interstitialReady;

        private bool _rewardedReady;
        private bool _interstitialReady;

        public void Initialize()
        {
#if ADMOB_SDK
            GoogleMobileAds.Api.MobileAds.Initialize(_ =>
            {
                IsInitialized = true;
            });
#else
            Debug.LogWarning("AdMob plugin not installed. Ads are disabled. Import Google Mobile Ads Unity plugin and add define ADMOB_SDK.");
            IsInitialized = true;
#endif
        }

        public void LoadRewarded()
        {
#if ADMOB_SDK
            var request = new GoogleMobileAds.Api.AdRequest();
            GoogleMobileAds.Api.RewardedAd.Load(GetRewardedUnitId(), request, (ad, error) =>
            {
                _rewarded = ad;
                _rewardedReady = ad != null && error == null;
                if (_rewarded != null)
                {
                    _rewarded.OnAdFullScreenContentClosed += () => { _rewardedReady = false; };
                    _rewarded.OnAdFullScreenContentFailed += _ => { _rewardedReady = false; };
                }
            });
#else
            _rewardedReady = false;
#endif
        }

        public void LoadInterstitial()
        {
#if ADMOB_SDK
            var request = new GoogleMobileAds.Api.AdRequest();
            GoogleMobileAds.Api.InterstitialAd.Load(GetInterstitialUnitId(), request, (ad, error) =>
            {
                _interstitial = ad;
                _interstitialReady = ad != null && error == null;
                if (_interstitial != null)
                {
                    _interstitial.OnAdFullScreenContentClosed += () => { _interstitialReady = false; };
                    _interstitial.OnAdFullScreenContentFailed += _ => { _interstitialReady = false; };
                }
            });
#else
            _interstitialReady = false;
#endif
        }

        public void ShowRewarded(string placement, Action onReward, Action onClosed = null, Action<string> onFailed = null)
        {
#if ADMOB_SDK
            if (_rewarded == null || !_rewarded.CanShowAd())
            {
                onFailed?.Invoke("Rewarded not ready");
                return;
            }

            _rewarded.Show(_ =>
            {
                onReward?.Invoke();
            });

            onClosed?.Invoke();
#else
            onFailed?.Invoke("AdMob not enabled");
            onClosed?.Invoke();
#endif
        }

        public void ShowInterstitial(string placement, Action onClosed = null, Action<string> onFailed = null)
        {
#if ADMOB_SDK
            if (_interstitial == null || !_interstitial.CanShowAd())
            {
                onFailed?.Invoke("Interstitial not ready");
                return;
            }

            _interstitial.Show();
            onClosed?.Invoke();
#else
            onFailed?.Invoke("AdMob not enabled");
            onClosed?.Invoke();
#endif
        }

#if ADMOB_SDK
        private GoogleMobileAds.Api.RewardedAd _rewarded;
        private GoogleMobileAds.Api.InterstitialAd _interstitial;
#endif

        private static string GetRewardedUnitId()
        {
#if UNITY_IOS
            // Test ad unit id from Google samples
            return "ca-app-pub-3940256099942544/1712485313";
#else
            return "ca-app-pub-3940256099942544/5224354917";
#endif
        }

        private static string GetInterstitialUnitId()
        {
#if UNITY_IOS
            return "ca-app-pub-3940256099942544/4411468910";
#else
            return "ca-app-pub-3940256099942544/1033173712";
#endif
        }
    }
}

