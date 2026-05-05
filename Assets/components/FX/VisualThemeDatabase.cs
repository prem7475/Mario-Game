using MarioGame.Levels;
using UnityEngine;

namespace MarioGame.Components.FX
{
    [CreateAssetMenu(menuName = "MarioGame/Visuals/Visual Theme Database", fileName = "VisualThemeDatabase")]
    public sealed class VisualThemeDatabase : ScriptableObject
    {
        public WorldVisualTheme[] worlds;

        public WorldVisualTheme Get(WorldThemeId id)
        {
            if (worlds == null) return null;
            for (var i = 0; i < worlds.Length; i++)
            {
                if (worlds[i] != null && worlds[i].world == id)
                    return worlds[i];
            }
            return null;
        }
    }
}

