# Unity Tilemap Setup (This Project)

This project supports **tile-based authored levels** via `LevelDefinition` ScriptableObjects and a runtime Tilemap builder.

## 1) Create authored levels (first 5 included)
In the Unity Editor:
- Menu: **MarioGame → Levels → Create LevelDatabase + First 5 Levels**

This creates:
- `Assets/assets/Levels/Resources/Levels/LevelDatabase.asset`
- `Assets/assets/Levels/Level_001.asset` … `Level_005.asset`

When `LevelDatabase.asset` exists, the game automatically loads authored levels (falls back to procedural when missing).

## 2) Tile coordinates vs world coordinates
- Tiles are in **tile coordinates** (`RectInt`), where 1 tile = 1 Unity unit.
- Spawn points (player/goal/coins/enemies/platforms) are in **world coordinates** (`Vector2`).

## 3) Tiles supported
In `LevelDefinition.TileRect.tileId`:
- `Solid`: ground/walls (collidable)
- `Breakable`: breakable blocks (requires Strawberry and upward hit)
- `Spike`: damage trigger
- `Fire`: timed on/off damage trigger

The runtime palette is defined in `Assets/levels/Tiles/RuntimeTilePalette.cs`.

## 4) Extending for real art tiles
Replace runtime tiles with real tiles by:
- Creating Tile assets in Unity (Tile Palette)
- Adding a `TilePalette` ScriptableObject that maps `TileId -> TileBase`
- Updating `RuntimeTilePalette` usage in `LevelLoader`

