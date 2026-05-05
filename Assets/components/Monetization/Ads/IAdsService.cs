using System;

namespace MarioGame.Components.Monetization.Ads
{
    public interface IAdsService
    {
        bool IsInitialized { get; }
        bool IsRewardedReady { get; }
        bool IsInterstitialReady { get; }

        void Initialize();
        void LoadRewarded();
        void LoadInterstitial();

        void ShowRewarded(string placement, Action onReward, Action onClosed = null, Action<string> onFailed = null);
        void ShowInterstitial(string placement, Action onClosed = null, Action<string> onFailed = null);
    }
}

