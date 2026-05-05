using System;
using MarioGame.Components.Audio;
using MarioGame.Components.Camera;
using MarioGame.Components.FX;
using MarioGame.Components.Gameplay;
using MarioGame.Components.Gameplay.Enemies;
using MarioGame.Components.Gameplay.Environment;
using MarioGame.Components.Gameplay.Pickups;
using MarioGame.Components.Gameplay.Traps;
using MarioGame.Components.Input;
using MarioGame.Components.Player;
using MarioGame.Levels.Data;
using MarioGame.Utils.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace MarioGame.Levels
{
    public sealed class LevelRuntime : MonoBehaviour
    {
        public static LevelRuntime Current { get; private set; }

        private LevelProgression _progression;

        private int _levelNumber = 1;
        private float _time;

        private Vector3 _checkpointPosition;
        private Transform _player;
        private Health _playerHealth;
        private PlayerPowerState _playerPower;
        private FollowCamera2D _cameraFollow;
        private Canvas _hudCanvas;
        private Text _hudText;
        private bool _paused;
        private GameObject _levelSelectPanel;

        public int LevelNumber => _levelNumber;
        public float ElapsedTime => _time;
        public Health PlayerHealth => _playerHealth;

        public void Construct(LevelProgression progression)
        {
            _progression = progression;
        }

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            // Level loading is controlled by GameManager.
        }

        private void Update()
        {
            if (_paused)
                return;

            _time += Time.deltaTime;
            UpdateHud();

            // Keyboard convenience for editor
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                MarioGame.Scenes.GameFlow.GameManager.Instance?.TogglePause();
        }

        public void LoadLevel(int levelNumber)
        {
            _levelNumber = Mathf.Clamp(levelNumber, 1, 100);
            _time = 0f;
            CoinManager.Instance?.ResetForLevel();

            ClearLevelObjects();

            var theme = WorldTheme.ForLevel(_levelNumber);
            if (UnityEngine.Camera.main != null)
                UnityEngine.Camera.main.backgroundColor = WorldTheme.SkyColor(theme);

            _cameraFollow = EnsureCameraFollow();
            EnsureHud();

            GenerateLevel(_levelNumber, theme);
            SpawnPlayerAt(new Vector3(0f, 2f, 0f));
            _checkpointPosition = _player.position;
        }

        public void AddCoins(int amount)
        {
            CoinManager.Instance?.AddCoins(amount);
        }

        public void SetCheckpoint(Vector3 position)
        {
            _checkpointPosition = position;
        }

        public void RespawnAtCheckpoint()
        {
            if (_player == null)
                return;

            _player.position = _checkpointPosition;
            var rb = _player.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.velocity = Vector2.zero;
        }

        public void GameOver()
        {
            AudioService.PlaySfx(SfxId.GameOver);
            Time.timeScale = 0f;
            _paused = true;
            MarioGame.Components.UI.UIManager.Instance?.ShowGameOver(true);
        }

        public void CompleteLevel()
        {
            var stars = ComputeStars();
            var coins = CoinManager.Instance != null ? CoinManager.Instance.Coins : 0;
            _progression?.UpdateRecord(_levelNumber, stars, coins, _time);
            _progression?.UnlockNext(_levelNumber);
            LoadLevel(Mathf.Min(_levelNumber + 1, 100));
        }

        public void RestartLevel()
        {
            MarioGame.Components.UI.UIManager.Instance?.ShowGameOver(false);
            Time.timeScale = 1f;
            _paused = false;
            LoadLevel(_levelNumber);
        }

        public void TogglePause()
        {
            MarioGame.Scenes.GameFlow.GameManager.Instance?.TogglePause();
        }

        private int ComputeStars()
        {
            var totalCoins = CoinManager.Instance != null ? CoinManager.Instance.TotalCoins : 0;
            if (totalCoins <= 0)
                return 1;

            var coins = CoinManager.Instance != null ? CoinManager.Instance.Coins : 0;
            var ratio = (float)coins / totalCoins;
            if (ratio >= 0.9f)
                return 3;
            if (ratio >= 0.6f)
                return 2;
            return 1;
        }

        private FollowCamera2D EnsureCameraFollow()
        {
            var cam = UnityEngine.Camera.main;
            if (cam == null)
                throw new Exception("Main Camera missing.");

            var follow = cam.GetComponent<FollowCamera2D>();
            if (follow == null)
                follow = cam.gameObject.AddComponent<FollowCamera2D>();

            return follow;
        }

        private void EnsureHud()
        {
            if (_hudCanvas != null)
                return;

            var go = new GameObject("HUD");
            DontDestroyOnLoad(go);
            _hudCanvas = go.AddComponent<Canvas>();
            _hudCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            go.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            go.AddComponent<GraphicRaycaster>();

            var txtGo = new GameObject("HUDText");
            txtGo.transform.SetParent(go.transform, false);
            var rect = txtGo.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0, 1);
            rect.anchorMax = new Vector2(0, 1);
            rect.pivot = new Vector2(0, 1);
            rect.anchoredPosition = new Vector2(18, -16);
            rect.sizeDelta = new Vector2(800, 160);
            _hudText = txtGo.AddComponent<Text>();
            _hudText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            _hudText.fontSize = 26;
            _hudText.color = Color.white;
            _hudText.alignment = TextAnchor.UpperLeft;

            CreateHudButton(go.transform, "Pause", new Vector2(-18, -18), new Vector2(160, 64), anchorTopRight: true, onClick: () => MarioGame.Scenes.GameFlow.GameManager.Instance?.TogglePause());
            CreateHudButton(go.transform, "Restart", new Vector2(-18, -94), new Vector2(160, 64), anchorTopRight: true, onClick: () => MarioGame.Scenes.GameFlow.GameManager.Instance?.RestartLevel());
            CreateHudButton(go.transform, "Sound", new Vector2(-18, -170), new Vector2(160, 64), anchorTopRight: true, onClick: ToggleSound);
            CreateHudButton(go.transform, "Levels", new Vector2(-18, -246), new Vector2(160, 64), anchorTopRight: true, onClick: ToggleLevelSelect);

            _levelSelectPanel = CreateLevelSelectPanel(go.transform);
            _levelSelectPanel.SetActive(false);
        }

        private void ToggleSound()
        {
            if (_progression == null)
                return;

            _progression.SoundEnabled = !_progression.SoundEnabled;
            AudioService.SetSoundEnabled(_progression.SoundEnabled);
            UpdateHud(force: true);
        }

        private void ToggleLevelSelect()
        {
            if (_levelSelectPanel == null)
                return;

            var next = !_levelSelectPanel.activeSelf;
            if (next)
                RefreshLevelSelectPanel();
            _levelSelectPanel.SetActive(next);
            var gm = MarioGame.Scenes.GameFlow.GameManager.Instance;
            if (gm != null)
            {
                gm.SetPaused(next);
                // Don't show the pause menu while level select is open.
                MarioGame.Components.UI.UIManager.Instance?.ShowPause(false);
            }
            _paused = next;
            UpdateHud(force: true);
        }

        private void CreateHudButton(Transform parent, string label, Vector2 anchoredPos, Vector2 size, bool anchorTopRight, Action onClick)
        {
            var go = new GameObject(label + "Button");
            go.transform.SetParent(parent, false);

            var rect = go.AddComponent<RectTransform>();
            rect.sizeDelta = size;
            if (anchorTopRight)
            {
                rect.anchorMin = new Vector2(1, 1);
                rect.anchorMax = new Vector2(1, 1);
                rect.pivot = new Vector2(1, 1);
            }
            else
            {
                rect.anchorMin = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.pivot = new Vector2(0, 1);
            }

            rect.anchoredPosition = anchoredPos;

            var img = go.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.28f);

            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(() => onClick?.Invoke());

            var txtGo = new GameObject("Text");
            txtGo.transform.SetParent(go.transform, false);
            var tr = txtGo.AddComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.offsetMin = Vector2.zero;
            tr.offsetMax = Vector2.zero;

            var t = txtGo.AddComponent<Text>();
            t.text = label;
            t.alignment = TextAnchor.MiddleCenter;
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.color = Color.white;
        }

        private GameObject CreateLevelSelectPanel(Transform parent)
        {
            var panel = new GameObject("LevelSelectPanel");
            panel.transform.SetParent(parent, false);

            var rect = panel.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(920, 640);

            var bg = panel.AddComponent<Image>();
            bg.color = new Color(0f, 0f, 0f, 0.6f);

            CreateHudButton(panel.transform, "Close", new Vector2(-18, -18), new Vector2(140, 56), anchorTopRight: true, onClick: ToggleLevelSelect);

            var titleGo = new GameObject("Title");
            titleGo.transform.SetParent(panel.transform, false);
            var titleRect = titleGo.AddComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0, 1);
            titleRect.anchorMax = new Vector2(1, 1);
            titleRect.pivot = new Vector2(0.5f, 1);
            titleRect.anchoredPosition = new Vector2(0, -18);
            titleRect.sizeDelta = new Vector2(0, 60);
            var title = titleGo.AddComponent<Text>();
            title.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            title.fontSize = 34;
            title.alignment = TextAnchor.MiddleCenter;
            title.color = Color.white;
            title.text = "Select Level";

            var scrollGo = new GameObject("Scroll");
            scrollGo.transform.SetParent(panel.transform, false);
            var scrollRect = scrollGo.AddComponent<RectTransform>();
            scrollRect.anchorMin = new Vector2(0, 0);
            scrollRect.anchorMax = new Vector2(1, 1);
            scrollRect.offsetMin = new Vector2(24, 24);
            scrollRect.offsetMax = new Vector2(-24, -92);
            scrollGo.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.06f);

            var sr = scrollGo.AddComponent<ScrollRect>();
            sr.horizontal = false;

            var viewport = new GameObject("Viewport");
            viewport.transform.SetParent(scrollGo.transform, false);
            var vpRect = viewport.AddComponent<RectTransform>();
            vpRect.anchorMin = Vector2.zero;
            vpRect.anchorMax = Vector2.one;
            vpRect.offsetMin = new Vector2(12, 12);
            vpRect.offsetMax = new Vector2(-12, -12);
            var vpImg = viewport.AddComponent<Image>();
            vpImg.color = new Color(1f, 1f, 1f, 0.02f);
            viewport.AddComponent<Mask>().showMaskGraphic = false;
            sr.viewport = vpRect;

            var content = new GameObject("Content");
            content.transform.SetParent(viewport.transform, false);
            var contentRect = content.AddComponent<RectTransform>();
            contentRect.anchorMin = new Vector2(0, 1);
            contentRect.anchorMax = new Vector2(1, 1);
            contentRect.pivot = new Vector2(0.5f, 1);
            contentRect.anchoredPosition = Vector2.zero;
            contentRect.sizeDelta = new Vector2(0, 1200);
            sr.content = contentRect;

            var grid = content.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(150, 76);
            grid.spacing = new Vector2(16, 16);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.childAlignment = TextAnchor.UpperCenter;
            content.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            for (var i = 1; i <= 100; i++)
            {
                CreateLevelButton(content.transform, i);
            }

            return panel;
        }

        private void CreateLevelButton(Transform parent, int level)
        {
            var go = new GameObject($"Level_{level}");
            go.transform.SetParent(parent, false);
            var img = go.AddComponent<Image>();
            img.color = new Color(0f, 0f, 0f, 0.22f);

            var btn = go.AddComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                ToggleLevelSelect();
                LoadLevel(level);
            });

            var labelGo = new GameObject("Text");
            labelGo.transform.SetParent(go.transform, false);
            var rect = labelGo.AddComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(8, 6);
            rect.offsetMax = new Vector2(-8, -6);

            var t = labelGo.AddComponent<Text>();
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.alignment = TextAnchor.MiddleCenter;
            t.color = Color.white;

            var entry = go.AddComponent<LevelButtonEntry>();
            entry.level = level;
            entry.button = btn;
            entry.text = t;

            // Populate initial state.
            ApplyLevelButtonState(entry);
        }

        private void RefreshLevelSelectPanel()
        {
            if (_levelSelectPanel == null)
                return;

            var entries = _levelSelectPanel.GetComponentsInChildren<LevelButtonEntry>(true);
            for (var i = 0; i < entries.Length; i++)
                ApplyLevelButtonState(entries[i]);
        }

        private void ApplyLevelButtonState(LevelButtonEntry entry)
        {
            if (entry == null || entry.button == null || entry.text == null)
                return;

            var unlocked = _progression != null && _progression.IsUnlocked(entry.level);
            entry.button.interactable = unlocked;

            if (!unlocked)
            {
                entry.text.color = new Color(1f, 1f, 1f, 0.35f);
                entry.text.text = $"Level {entry.level}\n(locked)";
                return;
            }

            entry.text.color = Color.white;
            var record = _progression.GetRecord(entry.level);
            var stars = record.bestStars <= 0 ? 0 : record.bestStars;
            entry.text.text = $"Level {entry.level}\nStars: {stars}";
        }

        private sealed class LevelButtonEntry : MonoBehaviour
        {
            public int level;
            public Button button;
            public Text text;
        }

        private void UpdateHud(bool force = false)
        {
            if (_hudText == null)
                return;

            var sound = _progression != null && _progression.SoundEnabled;
            var coins = CoinManager.Instance != null ? CoinManager.Instance.Coins : 0;
            var totalCoins = CoinManager.Instance != null ? CoinManager.Instance.TotalCoins : 0;
            _hudText.text =
                $"Level: {_levelNumber} / 100  ({WorldTheme.ForLevel(_levelNumber)})\n" +
                $"Coins: {coins}/{totalCoins}   Time: {_time:0.0}s\n" +
                $"Lives: {_playerHealth?.Lives ?? 0}   Strawberry: {(_playerPower != null && _playerPower.IsBig ? "ON" : "OFF")}\n" +
                $"Stars (on finish): {ComputeStars()}   Sound: {(sound ? "ON" : "OFF")}   {( _paused ? "[PAUSED]" : "")}";
        }

        private void ClearLevelObjects()
        {
            var root = GameObject.Find("LevelRoot");
            if (root != null)
                Destroy(root);

            if (_player != null)
                Destroy(_player.gameObject);
        }

        private void SpawnPlayerAt(Vector3 position)
        {
            var player = new GameObject("Player");
            player.transform.position = position;
            player.AddComponent<PlayerMarker>();

            var sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Square;
            sr.color = new Color(0.95f, 0.2f, 0.2f);
            sr.sortingOrder = 10;
            player.AddComponent<PlayerPowerState>();

            var rb = player.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.gravityScale = 3.4f;

            var col = player.AddComponent<CapsuleCollider2D>();
            col.size = new Vector2(0.8f, 1.3f);
            col.direction = CapsuleDirection2D.Vertical;

            _playerHealth = player.AddComponent<Health>();
            _playerPower = player.GetComponent<PlayerPowerState>();

            player.AddComponent<PlayerController>();

            _player = player.transform;
            _cameraFollow.SetTarget(_player);
        }

        private void GenerateLevel(int levelNumber, WorldThemeId theme)
        {
            var root = new GameObject("LevelRoot");

            var parallax = new GameObject("Parallax");
            parallax.transform.SetParent(root.transform, false);
            parallax.AddComponent<ParallaxBackground>().Build(theme);

            // Basic difficulty scaling
            var difficulty01 = Mathf.InverseLerp(1f, 100f, levelNumber);
            var length = Mathf.RoundToInt(Mathf.Lerp(60f, 140f, difficulty01));
            var hazardDensity = Mathf.Lerp(0.04f, 0.14f, difficulty01);
            var enemyDensity = Mathf.Lerp(0.05f, 0.16f, difficulty01);

            var authored = TryLoadAuthoredLevel(levelNumber);
            if (authored != null)
            {
                var boundsGo = new GameObject("LevelBounds");
                boundsGo.transform.SetParent(root.transform, false);
                var bounds = boundsGo.AddComponent<LevelBounds>();
                bounds.minX = authored.minX;
                bounds.maxX = authored.maxX;
                bounds.minY = authored.minY;
                bounds.maxY = authored.maxY;

                var loader = new LevelLoader(root.transform, authored.ResolvedTheme());
                loader.BuildFromDefinition(authored);

                if (authored.hasBoss)
                    CreateBoss(root.transform, authored.levelNumber, authored.maxX - 10f);

                return;
            }

            // Procedural fallback (still used until you author levels).
            var boundsGo2 = new GameObject("LevelBounds");
            boundsGo2.transform.SetParent(root.transform, false);
            var bounds2 = boundsGo2.AddComponent<LevelBounds>();
            bounds2.minX = -2f;
            bounds2.maxX = length + 10f;
            bounds2.minY = -4f;
            bounds2.maxY = 14f;

            CreateGround(root.transform, theme, length);
            CreatePlatforms(root.transform, theme, levelNumber, length, hazardDensity);
            CreateCoins(root.transform, levelNumber, length);
            CreateEnemies(root.transform, levelNumber, length, enemyDensity);
            CreateCheckpoints(root.transform, length);
            CreateGoal(root.transform, length + 6f);

            if (levelNumber % 10 == 0)
                CreateBoss(root.transform, levelNumber, length + 2f);

            CreateHiddenStrawberry(root.transform, levelNumber, length);
        }

        private static LevelDefinition TryLoadAuthoredLevel(int levelNumber)
        {
            // Drop a LevelDatabase at `Assets/assets/Levels/Resources/Levels/LevelDatabase.asset`
            // and authored levels referenced by it.
            var db = Resources.Load<LevelDatabase>("Levels/LevelDatabase");
            if (db == null)
                return null;

            var def = db.Get(levelNumber);
            return def;
        }

        private void CreateGround(Transform parent, WorldThemeId theme, int length)
        {
            var ground = new GameObject("Ground");
            ground.transform.SetParent(parent, false);
            ground.transform.position = new Vector3(length / 2f, -1.5f, 0f);

            var sr = ground.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Square;
            sr.drawMode = SpriteDrawMode.Sliced;
            sr.size = new Vector2(length, 3f);
            sr.color = WorldTheme.GroundColor(theme);
            sr.sortingOrder = 0;

            var col = ground.AddComponent<BoxCollider2D>();
            col.size = new Vector2(length, 3f);
        }

        private void CreatePlatforms(Transform parent, WorldThemeId theme, int levelNumber, int length, float hazardDensity)
        {
            var rng = new System.Random(levelNumber * 9973);
            var count = Mathf.RoundToInt(length * 0.12f);
            for (var i = 0; i < count; i++)
            {
                var x = (float)(rng.NextDouble() * (length - 8) + 6);
                var y = (float)(rng.NextDouble() * 5.5 + 0.2);
                var w = (float)(rng.NextDouble() * 3.5 + 2.2);

                var p = new GameObject($"Platform_{i}");
                p.transform.SetParent(parent, false);
                p.transform.position = new Vector3(x, y, 0f);

                var sr = p.AddComponent<SpriteRenderer>();
                sr.sprite = RuntimeSprites.Square;
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.size = new Vector2(w, 0.7f);
                sr.color = WorldTheme.GroundColor(theme) * new Color(1.15f, 1.15f, 1.15f);
                sr.sortingOrder = 0;

                var col = p.AddComponent<BoxCollider2D>();
                col.size = new Vector2(w, 0.7f);

                // Some platforms move
                if (rng.NextDouble() < 0.12)
                {
                    var mp = p.AddComponent<MovingPlatform>();
                    mp.localOffset = new Vector2((float)(rng.NextDouble() * 2.8 + 1.4), 0f);
                    mp.periodSeconds = (float)(rng.NextDouble() * 1.4 + 2.0);
                }

                // Some blocks are breakable (more common later)
                if (rng.NextDouble() < Mathf.InverseLerp(1f, 100f, levelNumber) * 0.18)
                {
                    var b = new GameObject($"Breakable_{i}");
                    b.transform.SetParent(parent, false);
                    b.transform.position = new Vector3(x + 0.6f, y + 1.2f, 0f);
                    var bsr = b.AddComponent<SpriteRenderer>();
                    bsr.sprite = RuntimeSprites.Square;
                    bsr.color = new Color(0.65f, 0.4f, 0.2f);
                    bsr.sortingOrder = 1;
                    var bcol = b.AddComponent<BoxCollider2D>();
                    bcol.size = new Vector2(0.9f, 0.9f);
                    b.AddComponent<BreakableBlock>();
                }

                // Occasional trap above a platform
                if (rng.NextDouble() < hazardDensity)
                {
                    var trap = new GameObject($"Spikes_{i}");
                    trap.transform.SetParent(parent, false);
                    trap.transform.position = new Vector3(x, y + 0.65f, 0f);
                    var tsr = trap.AddComponent<SpriteRenderer>();
                    tsr.sprite = RuntimeSprites.Square;
                    tsr.color = theme == WorldThemeId.Lava ? new Color(1f, 0.35f, 0.15f) : new Color(0.85f, 0.85f, 0.9f);
                    tsr.sortingOrder = 1;
                    var tcol = trap.AddComponent<BoxCollider2D>();
                    tcol.isTrigger = true;
                    tcol.size = new Vector2(Mathf.Min(1.8f, w), 0.3f);
                    trap.AddComponent<TrapDamage>();
                }
            }
        }

        private void CreateCoins(Transform parent, int levelNumber, int length)
        {
            var rng = new System.Random(levelNumber * 1103);
            var count = Mathf.RoundToInt(Mathf.Lerp(30f, 80f, Mathf.InverseLerp(1f, 100f, levelNumber)));
            for (var i = 0; i < count; i++)
            {
                var x = (float)(rng.NextDouble() * (length - 4) + 2);
                var y = (float)(rng.NextDouble() * 6 + 0.5);

                var c = new GameObject($"Coin_{i}");
                c.transform.SetParent(parent, false);
                c.transform.position = new Vector3(x, y, 0f);
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

        private void CreateEnemies(Transform parent, int levelNumber, int length, float enemyDensity)
        {
            var rng = new System.Random(levelNumber * 7919);
            var count = Mathf.RoundToInt(length * enemyDensity);
            for (var i = 0; i < count; i++)
            {
                var x = (float)(rng.NextDouble() * (length - 10) + 6);
                var isFlying = rng.NextDouble() < 0.25;

                if (!isFlying)
                {
                    var e = CreateEnemyBase(parent, $"WalkerEnemy_{i}", x, -0.2f, new Color(0.25f, 0.15f, 0.1f), addTouchDamage: false);
                    var patrol = e.AddComponent<EnemyAI>();
                    patrol.ConfigurePatrol(left: x - 1.6f, right: x + 1.6f, patrolSpeed: 1.6f + (levelNumber * 0.01f));
                }
                else
                {
                    var y = (float)(rng.NextDouble() * 4.5 + 1.5);
                    var e = CreateEnemyBase(parent, $"FlyingEnemy_{i}", x, y, new Color(0.3f, 0.25f, 0.55f));
                    var flyer = e.AddComponent<FlyingEnemy>();
                    flyer.Configure(amplitude: 0.65f, frequency: 1.6f + (levelNumber * 0.01f), driftSpeed: 0.8f);
                }
            }
        }

        private GameObject CreateEnemyBase(Transform parent, string name, float x, float y, Color color, bool addTouchDamage = true)
        {
            var e = new GameObject(name);
            e.transform.SetParent(parent, false);
            e.transform.position = new Vector3(x, y, 0f);

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

            if (addTouchDamage)
                e.AddComponent<EnemyDamageOnTouch>();
            return e;
        }

        private void CreateCheckpoints(Transform parent, int length)
        {
            var count = Mathf.Max(1, length / 40);
            for (var i = 1; i <= count; i++)
            {
                var x = i * (length / (count + 1f));
                var cp = new GameObject($"Checkpoint_{i}");
                cp.transform.SetParent(parent, false);
                cp.transform.position = new Vector3(x, 0.2f, 0f);
                var sr = cp.AddComponent<SpriteRenderer>();
                sr.sprite = RuntimeSprites.Square;
                sr.color = new Color(0.1f, 0.9f, 0.9f);
                sr.sortingOrder = 2;
                var col = cp.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                col.size = new Vector2(0.6f, 1.2f);
                cp.AddComponent<Checkpoint>();
            }
        }

        private void CreateGoal(Transform parent, float x)
        {
            var goal = new GameObject("Goal");
            goal.transform.SetParent(parent, false);
            goal.transform.position = new Vector3(x, 0.2f, 0f);

            var sr = goal.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Square;
            sr.color = new Color(0.95f, 0.95f, 0.95f);
            sr.sortingOrder = 2;
            var col = goal.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            col.size = new Vector2(0.9f, 2f);
            goal.AddComponent<GoalFlag>();
        }

        private void CreateBoss(Transform parent, int levelNumber, float x)
        {
            var boss = new GameObject("Boss");
            boss.transform.SetParent(parent, false);
            boss.transform.position = new Vector3(x, 1.2f, 0f);

            var sr = boss.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Square;
            sr.color = new Color(0.7f, 0.1f, 0.1f);
            sr.sortingOrder = 9;
            boss.transform.localScale = new Vector3(1.8f, 1.8f, 1f);

            var rb = boss.AddComponent<Rigidbody2D>();
            rb.gravityScale = 3.4f;
            rb.freezeRotation = true;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

            var col = boss.AddComponent<BoxCollider2D>();
            col.size = new Vector2(0.9f, 0.9f);

            var controller = boss.AddComponent<BossController>();
            controller.Configure(levelNumber);
        }

        private void CreateHiddenStrawberry(Transform parent, int levelNumber, int length)
        {
            // "Hidden area" heuristic: place a strawberry behind an invisible trigger wall high up.
            var rng = new System.Random(levelNumber * 3571 + 13);
            var x = (float)(rng.NextDouble() * (length - 12) + 8);
            var y = 6.8f;

            var wall = new GameObject("HiddenWall");
            wall.transform.SetParent(parent, false);
            wall.transform.position = new Vector3(x, y - 0.3f, 0f);
            var wallCol = wall.AddComponent<BoxCollider2D>();
            wallCol.isTrigger = false;
            wallCol.size = new Vector2(0.25f, 2f);
            var wallSr = wall.AddComponent<SpriteRenderer>();
            wallSr.sprite = RuntimeSprites.Square;
            wallSr.color = new Color(1f, 1f, 1f, 0.02f);
            wallSr.sortingOrder = 1;

            var s = new GameObject("Strawberry");
            s.transform.SetParent(parent, false);
            s.transform.position = new Vector3(x + 1.1f, y, 0f);
            var sr = s.AddComponent<SpriteRenderer>();
            sr.sprite = RuntimeSprites.Circle;
            sr.color = new Color(1f, 0.2f, 0.6f);
            sr.sortingOrder = 5;
            var col = s.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
            col.radius = 0.32f;
            s.AddComponent<StrawberryPickup>();
        }
    }
}
