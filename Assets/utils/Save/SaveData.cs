using System;
using UnityEngine.Serialization;

namespace MarioGame.Utils.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public int unlockedLevel = 1; // 1..100
        public bool musicEnabled = true;
        public bool sfxEnabled = true;

        // Shop / economy
        public int softCoins = 0;
        public int inventoryExtraLives = 0;
        public bool purchasedUnlockAllLevels = false;

        // Unity's JsonUtility doesn't support Dictionary, so we store an array.
        [FormerlySerializedAs("levelRecords")]
        public LevelRecordEntry[] levelRecordEntries = Array.Empty<LevelRecordEntry>();
    }

    [Serializable]
    public sealed class LevelRecord
    {
        public int bestStars;
        public int bestCoins;
        public float bestTimeSeconds = -1f;
    }

    [Serializable]
    public sealed class LevelRecordEntry
    {
        public int levelNumber;
        public LevelRecord record = new LevelRecord();
    }
}
