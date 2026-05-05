# Performance & Production Notes (Mobile)

## Pooling
- Coins and walker enemies are pooled to reduce instantiation spikes:
  - `Assets/utils/Pooling/ObjectPool.cs`
  - `Assets/utils/Pooling/PooledObject.cs`
  - `Assets/levels/LevelRuntime.cs`

## Physics
- Player uses overlap-box ground checks; keep ground colliders simple (Tilemap + CompositeCollider2D recommended).
- Prefer fewer, larger colliders instead of many small ones for static geometry.

## Rendering
- Keep sprite materials consistent to reduce draw calls.
- Use sprite atlases for production art (Unity SpriteAtlas) once you import real sprites.

## Audio
- Store clips in `Resources` (offline) and keep background music compressed appropriately for mobile.
- Per-world music supported via `AudioThemeDatabase` (Resources).

## Build
- Android: prefer AAB for store; IL2CPP; ARM64.
- iOS: IL2CPP; strip engine code if possible; test on device for performance.

