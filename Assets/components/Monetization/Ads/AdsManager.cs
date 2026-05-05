using System;
using MarioGame.Utils.Save;
using UnityEngine;
using MarioGame.Utils.Analytics;

namespace MarioGame.Components.Monetization.Ads
{
    public sealed class AdsManager : MonoBehaviour
    {
        public static AdsManager Instance { get; private set; }

        [Header("Policy")]
        [SerializeField] private int interstitialMinLevelsBetween = 2;

        private IAdsService _service;
        private int _levelsSinceInterstitial;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            _service = new AdMobAdsService(); // compile-safe stub if plugin isn't installed
            _service.Initialize();
            _service.LoadRewarded();
            _service.LoadInterstitial();
        }

        public void NotifyLevelCompleted()
        {
            _levelsSinceInterstitial++;
        }

        public bool ShouldShowInterstitial()
        {
            if (!_service.IsInterstitialReady)
                return false;
            if (_levelsSinceInterstitial < interstitialMinLevelsBetween)
                return false;
            return true;
        }

        public void ShowInterstitial(string placement, Action onClosed = null)
        {
            if (!ShouldShowInterstitial())
            {
                onClosed?.Invoke();
                return;
            }

            _levelsSinceInterstitial = 0;
            LocalAnalytics.Track("ad_interstitial_show");
            _service.ShowInterstitial(placement, onClosed, onFailed: _ => onClosed?.Invoke());
            _service.LoadInterstitial();
        }

        public void ShowRewarded(string placement, Action onReward, Action onClosed = null)
        {
            if (!_service.IsRewardedReady)
            {
                onClosed?.Invoke();
                return;
            }

            LocalAnalytics.Track("ad_rewarded_show");
            _service.ShowRewarded(placement, () =>
            {
                LocalAnalytics.Track("ad_rewarded_reward");
                onReward?.Invoke();
            }, onClosed, onFailed: _ => onClosed?.Invoke());
            _service.LoadRewarded();
        }
    }
}
