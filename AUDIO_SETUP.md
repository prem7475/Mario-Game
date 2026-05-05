# Audio Setup (Per-world music + SFX)

## SFX (already wired)
Place clips in `Resources`:
- `Assets/assets/Audio/Resources/Audio/SFX/jump.*`
- `Assets/assets/Audio/Resources/Audio/SFX/coin.*`
- `Assets/assets/Audio/Resources/Audio/SFX/powerup.*`
- `Assets/assets/Audio/Resources/Audio/SFX/enemyhit.*`
- `Assets/assets/Audio/Resources/Audio/SFX/gameover.*`

## World music (optional, premium)
1. Create an `AudioThemeDatabase` asset
2. Save it to:
   - `Assets/assets/Audio/Resources/Audio/AudioThemeDatabase.asset`
3. Assign a different `AudioClip` per world (Jungle/Desert/Ice/Lava/Underwater)

Runtime call happens during level load:
- `Assets/levels/LevelRuntime.cs`

If the database is missing, music falls back to:
- `Assets/assets/Audio/Resources/Audio/Music/bgm.*`
