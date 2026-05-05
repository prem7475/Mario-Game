using UnityEngine;

namespace MarioGame.Components.FX
{
    [CreateAssetMenu(menuName = "MarioGame/Visuals/Parallax Theme", fileName = "ParallaxTheme_Jungle")]
    public sealed class ParallaxTheme : ScriptableObject
    {
        [Header("Sprites (optional)")]
        public Sprite farSprite;
        public Sprite midSprite;
        public Sprite nearSprite;

        [Header("Fallback tints (used when sprites are missing)")]
        public Color farTint = new Color(1f, 1f, 1f, 0.15f);
        public Color midTint = new Color(1f, 1f, 1f, 0.22f);
        public Color nearTint = new Color(0.15f, 0.55f, 0.2f, 0.28f);

        [Header("Parallax")]
        [Range(0f, 1f)] public float farParallax = 0.15f;
        [Range(0f, 1f)] public float midParallax = 0.28f;
        [Range(0f, 1f)] public float nearParallax = 0.42f;

        [Header("Sizing")]
        public float tileWidth = 20f;
        public float farHeight = 5.2f;
        public float midHeight = 4.0f;
        public float nearHeight = 3.1f;
        public float farY = 2.0f;
        public float midY = 1.2f;
        public float nearY = 0.0f;
    }
}

