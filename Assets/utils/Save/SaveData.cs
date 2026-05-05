using System;
using UnityEngine.Serialization;

namespace MarioGame.Utils.Save
{
    [Serializable]
    public sealed class SaveData
    {
        public int unlockedLevel = 1; // 1..100
        public bool soundEnabled = true;

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
