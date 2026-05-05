using UnityEngine;

namespace MarioGame.Levels.Data
{
    [CreateAssetMenu(menuName = "MarioGame/Level Database", fileName = "LevelDatabase")]
    public sealed class LevelDatabase : ScriptableObject
    {
        // Index is levelNumber-1 (1..100)
        public LevelDefinition[] levels = new LevelDefinition[100];

        public LevelDefinition Get(int levelNumber)
        {
            if (levels == null || levels.Length == 0)
                return null;

            var idx = Mathf.Clamp(levelNumber - 1, 0, levels.Length - 1);
            return levels[idx];
        }
    }
}

