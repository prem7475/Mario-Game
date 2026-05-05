# Visual Upgrade Setup (Sprites, Animations, VFX)

This repo already supports:
- Parallax backgrounds (theme-able)
- Player Animator parameters (`Speed`, `Grounded`, `Jump`)
- Particle bursts (coin/powerup/enemy stomp)
- Fire traps (timed on/off)

## 1) Sprite folder structure (example)
Put art here:
- `Assets/assets/sprites/Player/Idle/idle_000.png` ...
- `Assets/assets/sprites/Player/Run/run_000.png` ...
- `Assets/assets/sprites/Player/Jump/jump_000.png` ...
- `Assets/assets/sprites/Player/Fall/fall_000.png` ...

Parallax:
- `Assets/assets/sprites/Parallax/Jungle/far.png`, `mid.png`, `near.png` (repeatable recommended)

## 2) Import + slice sprite sheets
For each PNG (or sprite sheet) in Unity Inspector:
- Texture Type: `Sprite (2D and UI)`
- Sprite Mode: `Multiple` (only for sheets)
- Filter Mode: `Bilinear`
- Compression: `Normal` (or `High Quality` for flagship devices)
- Pixels Per Unit: keep consistent across your art (e.g. 32 or 64)
- Open **Sprite Editor** -> Slice -> Apply

## 3) Create Animator Controller (Idle/Run/Jump/Fall)
Unity menu:
- `MarioGame -> Visuals -> Create Player Animator Controller`

This creates:
- `Assets/assets/animations/Player/Player.controller`
- clips under `Assets/assets/animations/Player/`

Assign it:
- Add `Animator` component to Player prefab (if you switch from runtime-spawned player to a prefab)
- Set Controller to `Player.controller`

`PlayerController` drives these params:
- Float `Speed`
- Bool `Grounded`
- Bool `Jump`

## 4) Parallax themes (per world)
Create assets:
- `ParallaxTheme_Jungle`, `ParallaxTheme_Desert`, ...
- `WorldVisualTheme` entries per world
- `VisualThemeDatabase` and save it to:
  - `Assets/assets/Visuals/Resources/Visuals/VisualThemeDatabase.asset`

At runtime, `ParallaxBackground` loads `Resources/Visuals/VisualThemeDatabase` automatically.

## 5) Particles/VFX hooks
Already active:
- coin sparkle: `Assets/components/Gameplay/Pickups/CoinPickup.cs:1`
- powerup burst: `Assets/components/Gameplay/Pickups/StrawberryPickup.cs:1`
- stomp burst: `Assets/components/Gameplay/Enemies/EnemyAI.cs:1`

Next polish step is swapping runtime placeholders for real sprites/materials.

