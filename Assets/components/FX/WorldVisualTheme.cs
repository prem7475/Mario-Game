using MarioGame.Levels;
using UnityEngine;

namespace MarioGame.Components.FX
{
    [CreateAssetMenu(menuName = "MarioGame/Visuals/World Visual Theme", fileName = "WorldVisualTheme")]
    public sealed class WorldVisualTheme : ScriptableObject
    {
        public WorldThemeId world;
        public ParallaxTheme parallax;

        [Header("Optional art overrides")]
        public Sprite playerSprite;
        public Sprite coinSprite;
    }
}

