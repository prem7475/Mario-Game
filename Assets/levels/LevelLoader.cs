using MarioGame.Components.Gameplay;
using MarioGame.Components.Gameplay.Enemies;
using MarioGame.Components.Gameplay.Environment;
using MarioGame.Components.Gameplay.Pickups;
using MarioGame.Components.Gameplay.Traps;
using MarioGame.Components.Player;
using MarioGame.Levels.Data;
using MarioGame.Levels.Tiles;
using MarioGame.Utils.Runtime;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace MarioGame.Levels
{
    public sealed class LevelLoader
    {
        private readonly Transform _root;
        private readonly WorldThemeId _theme;
        private readonly RuntimeTilePalette _palette;

        public LevelLoader(Transform levelRoot, WorldThemeId theme)
        {
            _root = levelRoot;
            _theme = theme;
            _palette = new RuntimeTilePalette(theme);
        }

        public void BuildFromDefinition(LevelDefinition def)
        {
            BuildTilemap(def);
            BuildSpawns(def);
        }

        private void BuildTilemap(LevelDefinition def)
        {
            var gridGo = new GameObject("Grid");
            gridGo.transform.SetParent(_root, false);
            var grid = gridGo.AddComponent<Grid>();
            grid.cellSize = Vector3.one; // 1 tile = 1 unit

            for (var i = 0; i < def.tileLayers.Length; i++)
            {
                var layer = def.tileLayers[i];
                if (layer == null)
                    continue;

                var mapGo = new GameObject(layer.name);
                mapGo.transform.SetParent(gridGo.transform, false);
                var tilemap = mapGo.AddComponent<Tilemap>();
                var renderer = mapGo.AddComponent<TilemapRenderer>();
                renderer.sortingOrder = layer.sortingOrder;

                // Build rectangles.
                for (var r = 0; r < layer.rects.Length; r++)
                {
                    var rect = layer.rects[r];
                    var tile = _palette.GetTile(rect.tileId);
                    if (tile == null)
                        continue;

                    for (var x = rect.rect.xMin; x < rect.rect.xMax; x++)
                    for (var y = rect.rect.yMin; y < rect.rect.yMax; y++)
                        tilemap.SetTile(new Vector3Int(x, y, 0), tile);

                    // For hazard tiles, also spawn colliders/traps as GameObjects so gameplay can trigger.
                    if (rect.tileId == TileId.Spike || rect.tileId == TileId.Fire || rect.tileId == TileId.Breakable)
                        SpawnHazardsForRect(rect, tilemap);
                }

                if (layer.isSolid)
                {
                    var collider = mapGo.AddComponent<TilemapCollider2D>();
                    collider.usedByComposite = true;
                    mapGo.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                    var composite = mapGo.AddComponent<CompositeCollider2D>();
                    composite.geometryType = CompositeCollider2D.GeometryType.Polygons;
                }
            }
        }

        private void SpawnHazardsForRect(TileRect rect, Tilemap tilemap)
        {
            // Spawn thin trigger colliders aligned to the rect to avoid per-tile GameObjects.
            var worldMin = tilemap.CellToWorld(new Vector3Int(rect.rect.xMin, rect.rect.yMin, 0));
            var worldMax = tilemap.CellToWorld(new Vector3Int(rect.rect.xMax, rect.rect.yMax, 0));
            var center = (worldMin + worldMax) * 0.5f;
            var size = new Vector2(Mathf.Abs(worldMax.x - worldMin.x), Mathf.Abs(worldMax.y - worldMin.y));

            var go = new GameObject(rect.tileId.ToString());
            go.transform.SetParent(_root, false);
            go.transform.position = new Vector3(center.x, center.y, 0f);

            var col = go.AddComponent<BoxCollider2D>();
            col.isTrigger = rect.tileId != TileId.Breakable;
            col.size = size;

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Square;
            sr.color = new Color(1f, 1f, 1f, 0f); // invisible gameplay volume

            if (rect.tileId == TileId.Breakable)
            {
                // Breakable block uses solid collider (not trigger)
                col.isTrigger = false;
                go.AddComponent<BreakableBlock>();
                sr.color = _palette.Breakable.color;
                sr.sortingOrder = 1;
            }
            else if (rect.tileId == TileId.Spike)
            {
                go.AddComponent<TrapDamage>();
            }
            else if (rect.tileId == TileId.Fire)
            {
                go.AddComponent<FireTrap>();
            }
        }

        private void BuildSpawns(LevelDefinition def)
        {
            // Coins
            for (var i = 0; i < def.spawns.coins.Length; i++)
                SpawnCoinPattern(def.spawns.coins[i]);

            // Moving platforms
            for (var i = 0; i < def.spawns.movingPlatforms.Length; i++)
            {
                var mp = def.spawns.movingPlatforms[i];
                var p = SpawnPlatform("MovingPlatform", mp.position, mp.size, WorldTheme.GroundColor(_theme));
                var mover = p.AddComponent<MovingPlatform>();
                mover.localOffset = mp.localOffset;
                mover.periodSeconds = mp.periodSeconds;
            }

            // Falling platforms
            for (var i = 0; i < def.spawns.fallingPlatforms.Length; i++)
            {
                var fp = def.spawns.fallingPlatforms[i];
                var p = SpawnPlatform("FallingPlatform", fp.position, fp.size, WorldTheme.GroundColor(_theme) * new Color(1.12f, 1.12f, 1.12f));
                var fall = p.AddComponent<FallingPlatform>();
                fall.delaySeconds = fp.delaySeconds;
                fall.respawnSeconds = fp.respawnSeconds;
            }

            // Enemies
            for (var i = 0; i < def.spawns.enemies.Length; i++)
            {
                var e = def.spawns.enemies[i];
                if (e.type == EnemyType.Walker)
                {
                    var go = SpawnEnemyBase("WalkerEnemy", e.position, new Color(0.25f, 0.15f, 0.1f));
                    var ai = go.AddComponent<EnemyAI>();
                    ai.ConfigurePatrol(e.patrolLeftX, e.patrolRightX, e.speed);
                }
                else
                {
                    var go = SpawnEnemyBase("FlyingEnemy", e.position, new Color(0.3f, 0.25f, 0.55f));
                    var flyer = go.AddComponent<FlyingEnemy>();
                    flyer.Configure(e.flyAmplitude, e.flyFrequency, e.flyDriftSpeed);
                }
            }

            // Checkpoints
            for (var i = 0; i < def.spawns.checkpoints.Length; i++)
            {
                var cp = new GameObject($"Checkpoint_{i + 1}");
                cp.transform.SetParent(_root, false);
                cp.transform.position = def.spawns.checkpoints[i].position;
                var sr = cp.AddComponent<SpriteRenderer>();
                sr.sprite = RuntimeSprites.Square;
                sr.color = new Color(0.1f, 0.9f, 0.9f);
                sr.sortingOrder = 2;
                var col = cp.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                col.size = new Vector2(0.6f, 1.2f);
                cp.AddComponent<Checkpoint>();
            }

            // Goal
            var goal = new GameObject("Goal");
            goal.transform.SetParent(_root, false);
            goal.transform.position = def.spawns.goal.position;
            var gsr = goal.AddComponent<SpriteRenderer>();
            gsr.sprite = RuntimeSprites.Square;
            gsr.color = new Color(0.95f, 0.95f, 0.95f);
            gsr.sortingOrder = 2;
            var gcol = goal.AddComponent<BoxCollider2D>();
            gcol.isTrigger = true;
            gcol.size = new Vector2(0.9f, 2f);
            goal.AddComponent<GoalFlag>();

            // Strawberry (hidden)
            if (def.spawns.strawberry != null && def.spawns.strawberry.enabled)
            {
                var s = new GameObject("Strawberry");
                s.transform.SetParent(_root, false);
                s.transform.position = def.spawns.strawberry.position;
                var ssr = s.AddComponent<SpriteRenderer>();
                ssr.sprite = RuntimeSprites.Circle;
                ssr.color = new Color(1f, 0.2f, 0.6f);
                ssr.sortingOrder = 5;
                var scol = s.AddComponent<CircleCollider2D>();
                scol.isTrigger = true;
                scol.radius = 0.32f;
                var pickup = s.AddComponent<StrawberryPickup>();
                // Set serialized fields via defaults; authoring can override on prefab later.
            }

            // Boss is spawned by LevelRuntime (so it can plug into completion flow).
        }

        private void SpawnCoinPattern(CoinPattern p)
        {
            if (p.count <= 0)
                return;

            for (var i = 0; i < p.count; i++)
            {
                var t = p.count == 1 ? 0f : (float)i / (p.count - 1);
                var pos = p.type switch
                {
                    CoinPatternType.Line => Vector2.Lerp(p.start, p.end, t),
                    CoinPatternType.Stair => new Vector2(Mathf.Lerp(p.start.x, p.end.x, t), Mathf.Lerp(p.start.y, p.end.y, t) + Mathf.Floor(i * 0.4f) * 0.35f),
                    CoinPatternType.Arc => ArcPoint(p.start, p.end, p.arcHeight, t),
                    _ => Vector2.Lerp(p.start, p.end, t)
                };

                var c = new GameObject("Coin");
                c.transform.SetParent(_root, false);
                c.transform.position = pos;
                var sr = c.AddComponent<SpriteRenderer>();
                sr.sprite = RuntimeSprites.Circle;
                sr.color = new Color(1f, 0.85f, 0.15f);
                sr.sortingOrder = 5;
                var col = c.AddComponent<CircleCollider2D>();
                col.isTrigger = true;
                col.radius = 0.28f;
                c.AddComponent<CoinPickup>();
                CoinManager.Instance?.RegisterCoinSpawned(1);
            }
        }

        private static Vector2 ArcPoint(Vector2 a, Vector2 b, float height, float t)
        {
            var mid = (a + b) * 0.5f + Vector2.up * height;
            // Quadratic Bezier: (1-t)^2 A + 2(1-t)t M + t^2 B
            var u = 1f - t;
            return (u * u) * a + (2f * u * t) * mid + (t * t) * b;
        }

        private GameObject SpawnPlatform(string name, Vector2 pos, Vector2 size, Color color)
        {
            var p = new GameObject(name);
            p.transform.SetParent(_root, false);
            p.transform.position = pos;
            var sr = p.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Square;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = size;
            sr.color = color;
            sr.sortingOrder = 0;
            var col = p.AddComponent<BoxCollider2D>();
            col.size = size;
            return p;
        }

        private GameObject SpawnEnemyBase(string name, Vector2 pos, Color color)
        {
            var e = new GameObject(name);
            e.transform.SetParent(_root, false);
            e.transform.position = pos;

            var sr = e.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Square;
            sr.color = color;
            sr.sortingOrder = 9;

            var rb = e.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3.4f;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = e.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.8f, 0.8f);

            return e;
        }
    }
}

