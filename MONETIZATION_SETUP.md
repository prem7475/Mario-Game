# Monetization Setup (AdMob + IAP + Shop)

This project is wired so it **compiles without monetization SDKs**, and turns them on once you install the packages.

## 1) AdMob (Google Mobile Ads Unity Plugin)

### A) Install the plugin
1. Download/import the **Google Mobile Ads Unity plugin** (AdMob).
2. After importing, add a Scripting Define Symbol:
   - `Edit -> Project Settings -> Player -> Other Settings -> Scripting Define Symbols`
   - Add: `ADMOB_SDK`

Without `ADMOB_SDK`, ads are disabled but the game still builds.

### B) Configure your App IDs
In the AdMob/Google Mobile Ads plugin settings, set:
- Android App ID
- iOS App ID

### C) Ad unit IDs
The code uses **Google test ad unit IDs** by default:
- Rewarded: `Assets/components/Monetization/Ads/AdMobAdsService.cs`
- Interstitial: `Assets/components/Monetization/Ads/AdMobAdsService.cs`

Replace them with your own ad unit IDs for production.

### D) Where ads are shown (UX-safe)
- Rewarded:
  - Shop: “Reward +1 Life”
  - Level Select: “Unlock” (reward unlocks next locked level)
- Interstitial:
  - Only when player taps **Next** on the level-complete screen (never mid-game)
  - Frequency limited by `interstitialMinLevelsBetween` in `Assets/components/Monetization/Ads/AdsManager.cs`

## 2) In-App Purchases (Unity IAP)

### A) Install Unity IAP
Unity: `Window -> Package Manager` -> install **In-App Purchasing**.

When installed, Unity defines `UNITY_PURCHASING` and enables:
- `Assets/components/Monetization/IAP/UnityIapImpl.cs`

### B) Product IDs (must match stores)
Defined in:
- `Assets/components/Monetization/IAP/ProductIds.cs`

Create these products in both stores:
- `lives_small` (Consumable) -> grants +5 extra lives
- `coins_small` (Consumable) -> grants +500 soft coins
- `unlock_all_levels` (Non-consumable) -> unlocks level 100

### C) Entitlements + local save
Entitlements are stored locally in:
- `Assets/utils/Save/SaveData.cs`

## 3) Shop UI
- Shop controller: `Assets/components/Monetization/Shop/ShopController.cs`
- Open from Pause Menu: `Shop` button

## 4) Analytics (offline/local)
Basic counters are stored in `PlayerPrefs`:
- `Assets/utils/Analytics/LocalAnalytics.cs`

You can later swap this out for Unity Analytics / Firebase / custom backend.

