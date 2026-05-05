using MarioGame.Utils.Save;

namespace MarioGame.Levels
{
    public sealed class LevelProgression
    {
        private readonly SaveService _save;

        public LevelProgression(SaveService save)
        {
            _save = save;
        }

        public int UnlockedLevel => ClampLevel(_save.Data.unlockedLevel);

        public int ClampLevel(int level) => level < 1 ? 1 : level > 100 ? 100 : level;

        public bool IsUnlocked(int level) => level <= UnlockedLevel;

        public void UnlockNext(int completedLevel)
        {
            var next = ClampLevel(completedLevel + 1);
            if (next > _save.Data.unlockedLevel)
            {
                _save.Data.unlockedLevel = next;
                _save.Save();
            }
        }

        public LevelRecord GetRecord(int level)
        {
            level = ClampLevel(level);
            var entries = _save.Data.levelRecordEntries ?? System.Array.Empty<LevelRecordEntry>();
            for (var i = 0; i < entries.Length; i++)
            {
                if (entries[i] != null && entries[i].levelNumber == level && entries[i].record != null)
                    return entries[i].record;
            }

            var newEntry = new LevelRecordEntry { levelNumber = level, record = new LevelRecord() };
            var newArr = new LevelRecordEntry[entries.Length + 1];
            for (var i = 0; i < entries.Length; i++) newArr[i] = entries[i];
            newArr[newArr.Length - 1] = newEntry;
            _save.Data.levelRecordEntries = newArr;
            _save.Save();
            return newEntry.record;
        }

        public void UpdateRecord(int level, int stars, int coins, float timeSeconds)
        {
            var record = GetRecord(level);
            record.bestStars = stars > record.bestStars ? stars : record.bestStars;
            record.bestCoins = coins > record.bestCoins ? coins : record.bestCoins;
            if (record.bestTimeSeconds < 0f || timeSeconds < record.bestTimeSeconds)
                record.bestTimeSeconds = timeSeconds;

            _save.Save();
        }

        public bool SoundEnabled
        {
            get => _save.Data.soundEnabled;
            set
            {
                _save.Data.soundEnabled = value;
                _save.Save();
            }
        }

        // retained intentionally for older save migrations if needed later
    }
}
