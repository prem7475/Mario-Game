using System.Collections.Generic;
using System.IO;
using MarioGame.Levels.Data;
using UnityEditor;
using UnityEngine;

namespace MarioGame.Levels.Editor
{
    public static class LevelAuthoringTools
    {
        private const string LevelsRoot = "Assets/assets/Levels";
        private const string ResourcesDbPath = "Assets/assets/Levels/Resources/Levels/LevelDatabase.asset";

        [MenuItem("MarioGame/Levels/Create LevelDatabase + First 5 Levels")]
        public static void CreateDatabaseAndFirstLevels()
        {
            Directory.CreateDirectory(LevelsRoot);
            Directory.CreateDirectory("Assets/assets/Levels/Resources/Levels");

            var db = AssetDatabase.LoadAssetAtPath<LevelDatabase>(ResourcesDbPath);
            if (db == null)
            {
                db = ScriptableObject.CreateInstance<LevelDatabase>();
                db.levels = new LevelDefinition[100];
                AssetDatabase.CreateAsset(db, ResourcesDbPath);
            }

            var first = BuildFirst5();
            for (var i = 0; i < first.Count; i++)
            {
                var def = first[i];
                var assetPath = $"{LevelsRoot}/Level_{def.levelNumber:000}.asset";
                var existing = AssetDatabase.LoadAssetAtPath<LevelDefinition>(assetPath);
                if (existing == null)
                {
                    AssetDatabase.CreateAsset(def, assetPath);
                    existing = def;
                }
                else
                {
                    EditorUtility.CopySerialized(def, existing);
                    UnityEngine.Object.DestroyImmediate(def);
                }

                db.levels[existing.levelNumber - 1] = existing;
                EditorUtility.SetDirty(existing);
            }

            EditorUtility.SetDirty(db);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = db;
        }

        private static List<LevelDefinition> BuildFirst5()
        {
            var list = new List<LevelDefinition>
            {
                Level01(),
                Level02(),
                Level03(),
                Level04(),
                Level05()
            };
            return list;
        }

        private static LevelDefinition Base(int n, float length)
        {
            var d = ScriptableObject.CreateInstance<LevelDefinition>();
            d.levelNumber = n;
            d.minX = -2f;
            d.maxX = length;
            d.minY = -4f;
            d.maxY = 14f;
            d.tileLayers = new[]
            {
                new TileLayer
                {
                    name = "Solid",
                    sortingOrder = 0,
                    isSolid = true,
                    rects = new[]
                    {
                        new TileRect { tileId = TileId.Solid, rect = new RectInt(0, -2, Mathf.RoundToInt(length), 2) }
                    }
                }
            };
            d.spawns = new SpawnGroup
            {
                playerStart = new SpawnPoint { position = new Vector2(2, 2) },
                goal = new SpawnPoint { position = new Vector2(length - 6f, 0.2f) },
                checkpoints = new[] { new SpawnPoint { position = new Vector2(length * 0.5f, 0.2f) } },
                coins = new CoinPattern[] { },
                enemies = new EnemySpawn[] { },
                movingPlatforms = new MovingPlatformSpawn[] { },
                fallingPlatforms = new FallingPlatformSpawn[] { },
                strawberry = new StrawberrySpawn { enabled = true, position = new Vector2(length * 0.65f, 7.2f), durationSeconds = 10f, extraLives = 1 }
            };
            return d;
        }

        // Level 1: tutorial run + 2 gentle jumps + coin line.
        private static LevelDefinition Level01()
        {
            var d = Base(1, length: 80f);
            d.spawns.coins = new[]
            {
                new CoinPattern{ type = CoinPatternType.Line, start = new Vector2(6, 1.5f), end = new Vector2(18, 1.5f), count = 8 },
                new CoinPattern{ type = CoinPatternType.Arc, start = new Vector2(26, 2.2f), end = new Vector2(34, 2.2f), count = 7, arcHeight = 2.0f },
                new CoinPattern{ type = CoinPatternType.Line, start = new Vector2(48, 3.6f), end = new Vector2(56, 3.6f), count = 6 }
            };

            // Platforms for gentle jumps
            d.spawns.movingPlatforms = new MovingPlatformSpawn[] { };
            d.spawns.fallingPlatforms = new FallingPlatformSpawn[] { };

            d.tileLayers[0].rects = new[]
            {
                new TileRect { tileId = TileId.Solid, rect = new RectInt(0, -2, 80, 2) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(22, 0, 4, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(34, 1, 4, 1) },
                new TileRect { tileId = TileId.Breakable, rect = new RectInt(40, 2, 2, 2) }
            };

            // One basic walker
            d.spawns.enemies = new[]
            {
                new EnemySpawn{ type = EnemyType.Walker, position = new Vector2(30, -0.2f), patrolLeftX = 28, patrolRightX = 34, speed = 1.5f }
            };

            // Strawberry: hidden above breakables
            d.spawns.strawberry.position = new Vector2(41, 6.8f);
            return d;
        }

        // Level 2: introduce spikes + coin arc over hazard.
        private static LevelDefinition Level02()
        {
            var d = Base(2, length: 86f);
            d.tileLayers[0].rects = new[]
            {
                new TileRect { tileId = TileId.Solid, rect = new RectInt(0, -2, 86, 2) },
                new TileRect { tileId = TileId.Spike, rect = new RectInt(24, -1, 4, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(22, 0, 2, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(28, 0, 2, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(44, 1, 5, 1) }
            };

            d.spawns.coins = new[]
            {
                new CoinPattern{ type = CoinPatternType.Arc, start = new Vector2(22.5f, 2.4f), end = new Vector2(29.5f, 2.4f), count = 8, arcHeight = 2.4f },
                new CoinPattern{ type = CoinPatternType.Line, start = new Vector2(46, 3.2f), end = new Vector2(52, 3.2f), count = 6 }
            };

            d.spawns.enemies = new[]
            {
                new EnemySpawn{ type = EnemyType.Walker, position = new Vector2(54, -0.2f), patrolLeftX = 52, patrolRightX = 60, speed = 1.6f }
            };

            d.spawns.strawberry.position = new Vector2(12, 7.6f); // early hidden jump
            return d;
        }

        // Level 3: introduce moving platform + flying enemy.
        private static LevelDefinition Level03()
        {
            var d = Base(3, length: 96f);
            d.tileLayers[0].rects = new[]
            {
                new TileRect { tileId = TileId.Solid, rect = new RectInt(0, -2, 96, 2) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(18, 1, 4, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(32, 2, 4, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(54, 1, 4, 1) },
                new TileRect { tileId = TileId.Breakable, rect = new RectInt(66, 2, 3, 2) }
            };

            d.spawns.movingPlatforms = new[]
            {
                new MovingPlatformSpawn{ position = new Vector2(42, 2.2f), size = new Vector2(3.5f, 0.7f), localOffset = new Vector2(3.2f, 0f), periodSeconds = 2.4f }
            };

            d.spawns.coins = new[]
            {
                new CoinPattern{ type = CoinPatternType.Line, start = new Vector2(18.5f, 3.0f), end = new Vector2(22.5f, 3.0f), count = 4 },
                new CoinPattern{ type = CoinPatternType.Arc, start = new Vector2(40, 4.4f), end = new Vector2(48, 4.4f), count = 8, arcHeight = 1.8f },
                new CoinPattern{ type = CoinPatternType.Line, start = new Vector2(66, 4.0f), end = new Vector2(72, 4.0f), count = 6 }
            };

            d.spawns.enemies = new[]
            {
                new EnemySpawn{ type = EnemyType.Flying, position = new Vector2(50, 3.6f), flyAmplitude = 0.7f, flyFrequency = 1.8f, flyDriftSpeed = 0.6f },
                new EnemySpawn{ type = EnemyType.Walker, position = new Vector2(74, -0.2f), patrolLeftX = 72, patrolRightX = 84, speed = 1.7f }
            };

            d.spawns.strawberry.position = new Vector2(67, 7.4f);
            return d;
        }

        // Level 4: introduce falling platforms + hidden coin stair.
        private static LevelDefinition Level04()
        {
            var d = Base(4, length: 104f);
            d.tileLayers[0].rects = new[]
            {
                new TileRect { tileId = TileId.Solid, rect = new RectInt(0, -2, 104, 2) },
                new TileRect { tileId = TileId.Spike, rect = new RectInt(34, -1, 6, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(48, 1, 4, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(78, 1, 4, 1) }
            };

            d.spawns.fallingPlatforms = new[]
            {
                new FallingPlatformSpawn{ position = new Vector2(26, 2.0f), size = new Vector2(2.8f, 0.7f), delaySeconds = 0.25f },
                new FallingPlatformSpawn{ position = new Vector2(30, 3.2f), size = new Vector2(2.6f, 0.7f), delaySeconds = 0.25f },
                new FallingPlatformSpawn{ position = new Vector2(42, 3.2f), size = new Vector2(2.6f, 0.7f), delaySeconds = 0.2f }
            };

            d.spawns.coins = new[]
            {
                new CoinPattern{ type = CoinPatternType.Stair, start = new Vector2(14, 1.8f), end = new Vector2(22, 3.2f), count = 9 },
                new CoinPattern{ type = CoinPatternType.Arc, start = new Vector2(33, 2.4f), end = new Vector2(41, 2.4f), count = 8, arcHeight = 2.2f },
                new CoinPattern{ type = CoinPatternType.Line, start = new Vector2(78, 3.2f), end = new Vector2(86, 3.2f), count = 8 }
            };

            d.spawns.enemies = new[]
            {
                new EnemySpawn{ type = EnemyType.Walker, position = new Vector2(60, -0.2f), patrolLeftX = 58, patrolRightX = 68, speed = 1.7f }
            };

            d.spawns.strawberry.position = new Vector2(6, 7.8f); // very hidden early
            return d;
        }

        // Level 5: introduce fire trap timing + moving platform combo.
        private static LevelDefinition Level05()
        {
            var d = Base(5, length: 116f);
            d.tileLayers[0].rects = new[]
            {
                new TileRect { tileId = TileId.Solid, rect = new RectInt(0, -2, 116, 2) },
                new TileRect { tileId = TileId.Fire, rect = new RectInt(28, -1, 4, 1) },
                new TileRect { tileId = TileId.Spike, rect = new RectInt(56, -1, 4, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(26, 0, 2, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(32, 0, 2, 1) },
                new TileRect { tileId = TileId.Solid, rect = new RectInt(52, 1, 4, 1) },
                new TileRect { tileId = TileId.Breakable, rect = new RectInt(90, 2, 3, 2) }
            };

            d.spawns.movingPlatforms = new[]
            {
                new MovingPlatformSpawn{ position = new Vector2(42, 2.4f), size = new Vector2(3.4f, 0.7f), localOffset = new Vector2(4.0f, 0f), periodSeconds = 2.2f }
            };

            d.spawns.coins = new[]
            {
                new CoinPattern{ type = CoinPatternType.Arc, start = new Vector2(26.5f, 2.4f), end = new Vector2(33.5f, 2.4f), count = 8, arcHeight = 2.0f },
                new CoinPattern{ type = CoinPatternType.Line, start = new Vector2(42, 4.7f), end = new Vector2(50, 4.7f), count = 7 },
                new CoinPattern{ type = CoinPatternType.Arc, start = new Vector2(54, 3.2f), end = new Vector2(62, 3.2f), count = 8, arcHeight = 2.2f }
            };

            d.spawns.enemies = new[]
            {
                new EnemySpawn{ type = EnemyType.Walker, position = new Vector2(70, -0.2f), patrolLeftX = 68, patrolRightX = 82, speed = 1.8f },
                new EnemySpawn{ type = EnemyType.Flying, position = new Vector2(84, 3.6f), flyAmplitude = 0.75f, flyFrequency = 1.9f, flyDriftSpeed = 0.5f }
            };

            d.spawns.strawberry.position = new Vector2(91.5f, 7.4f);
            return d;
        }
    }
}

