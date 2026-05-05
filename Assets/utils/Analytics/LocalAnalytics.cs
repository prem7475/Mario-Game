using UnityEngine;

namespace MarioGame.Utils.Analytics
{
    public static class LocalAnalytics
    {
        private const string Prefix = "mariogame.analytics.";

        public static void Increment(string key, int by = 1)
        {
            var k = Prefix + key;
            var v = PlayerPrefs.GetInt(k, 0);
            v += by;
            PlayerPrefs.SetInt(k, v);
        }

        public static void Track(string key)
        {
            Increment(key, 1);
            Debug.Log($"[Analytics] {key}");
        }
    }
}

