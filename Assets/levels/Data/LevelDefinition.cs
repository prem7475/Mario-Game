using System;
using UnityEngine;

namespace MarioGame.Levels.Data
{
    [CreateAssetMenu(menuName = "MarioGame/Level Definition", fileName = "Level_001")]
    public sealed class LevelDefinition : ScriptableObject
    {
        [Range(1, 100)] public int levelNumber = 1;
        public WorldThemeId themeOverride = 0; // 0 = auto

        [Header("Bounds (world units)")]
        public float minX = -2f;
        public float maxX = 120f;
        public float minY = -4f;
        public float maxY = 14f;

        [Header("Layout")]
        public TileLayer[] tileLayers = Array.Empty<TileLayer>();
        public SpawnGroup spawns = new SpawnGroup();

        [Header("Rules")]
        public bool hasBoss;
        public int bossHp = 6;

        public WorldThemeId ResolvedTheme()
        {
            if (themeOverride != 0)
                return themeOverride;
            return WorldTheme.ForLevel(levelNumber);
        }
    }

    [Serializable]
    public sealed class TileLayer
    {
        public string name = "Ground";
        public int sortingOrder = 0;
        public bool isSolid = true;
        public TileRect[] rects = Array.Empty<TileRect>();
    }

    [Serializable]
    public sealed class TileRect
    {
        public RectInt rect; // tile coordinates
        public TileId tileId = TileId.Solid;
    }

    public enum TileId
    {
        Empty = 0,
        Solid = 1,
        Breakable = 2,
        Spike = 3,
        Fire = 4
    }

    [Serializable]
    public sealed class SpawnGroup
    {
        public SpawnPoint playerStart = new SpawnPoint { position = new Vector2(0, 2) };
        public SpawnPoint goal = new SpawnPoint { position = new Vector2(70, 0.2f) };
        public SpawnPoint[] checkpoints = Array.Empty<SpawnPoint>();

        public CoinPattern[] coins = Array.Empty<CoinPattern>();
        public EnemySpawn[] enemies = Array.Empty<EnemySpawn>();
        public MovingPlatformSpawn[] movingPlatforms = Array.Empty<MovingPlatformSpawn>();
        public FallingPlatformSpawn[] fallingPlatforms = Array.Empty<FallingPlatformSpawn>();
        public StrawberrySpawn strawberry = new StrawberrySpawn { enabled = true, position = new Vector2(40, 7) };
    }

    [Serializable]
    public sealed class SpawnPoint
    {
        public Vector2 position;
    }

    public enum CoinPatternType
    {
        Line,
        Arc,
        Stair
    }

    [Serializable]
    public sealed class CoinPattern
    {
        public CoinPatternType type = CoinPatternType.Line;
        public Vector2 start;
        public Vector2 end;
        public int count = 8;
        public float arcHeight = 2.2f; // used by Arc
    }

    public enum EnemyType
    {
        Walker,
        Flying
    }

    [Serializable]
    public sealed class EnemySpawn
    {
        public EnemyType type = EnemyType.Walker;
        public Vector2 position;
        public float patrolLeftX;
        public float patrolRightX;
        public float speed = 1.6f;

        // flying options
        public float flyAmplitude = 0.65f;
        public float flyFrequency = 1.6f;
        public float flyDriftSpeed = 0.8f;
    }

    [Serializable]
    public sealed class MovingPlatformSpawn
    {
        public Vector2 position;
        public Vector2 localOffset = new Vector2(2.5f, 0f);
        public float periodSeconds = 2.6f;
        public Vector2 size = new Vector2(3f, 0.7f);
    }

    [Serializable]
    public sealed class FallingPlatformSpawn
    {
        public Vector2 position;
        public Vector2 size = new Vector2(2.6f, 0.7f);
        public float delaySeconds = 0.25f;
        public float respawnSeconds = 999f; // set small if you want respawn in-level
    }

    [Serializable]
    public sealed class StrawberrySpawn
    {
        public bool enabled = true;
        public Vector2 position;
        public float durationSeconds = 10f;
        public int extraLives = 1;
    }
}

