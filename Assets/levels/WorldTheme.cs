using UnityEngine;

namespace MarioGame.Levels
{
    public enum WorldThemeId
    {
        Jungle = 1,
        Desert = 2,
        Ice = 3,
        Lava = 4,
        Underwater = 5
    }

    public static class WorldTheme
    {
        public static WorldThemeId ForLevel(int levelNumber)
        {
            // 1..20 Jungle, 21..40 Desert, 41..60 Ice, 61..80 Lava, 81..100 Underwater
            return (WorldThemeId)(Mathf.Clamp((levelNumber - 1) / 20 + 1, 1, 5));
        }

        public static Color SkyColor(WorldThemeId id)
        {
            return id switch
            {
                WorldThemeId.Jungle => new Color(0.55f, 0.85f, 1f),
                WorldThemeId.Desert => new Color(0.95f, 0.82f, 0.55f),
                WorldThemeId.Ice => new Color(0.75f, 0.9f, 1f),
                WorldThemeId.Lava => new Color(0.35f, 0.1f, 0.12f),
                WorldThemeId.Underwater => new Color(0.12f, 0.25f, 0.55f),
                _ => new Color(0.55f, 0.85f, 1f)
            };
        }

        public static Color GroundColor(WorldThemeId id)
        {
            return id switch
            {
                WorldThemeId.Jungle => new Color(0.2f, 0.55f, 0.2f),
                WorldThemeId.Desert => new Color(0.78f, 0.65f, 0.35f),
                WorldThemeId.Ice => new Color(0.75f, 0.86f, 0.92f),
                WorldThemeId.Lava => new Color(0.35f, 0.25f, 0.22f),
                WorldThemeId.Underwater => new Color(0.1f, 0.3f, 0.38f),
                _ => new Color(0.2f, 0.55f, 0.2f)
            };
        }
    }
}

