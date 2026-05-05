# Level System (100 Levels)

This project uses a **deterministic procedural generator** so you get **100 playable levels** without manually authoring 100 scenes.

## Worlds
- Levels **1–20**: Jungle
- Levels **21–40**: Desert
- Levels **41–60**: Ice
- Levels **61–80**: Lava
- Levels **81–100**: Underwater

World selection is implemented in `Assets/levels/WorldTheme.cs`.

## Boss Levels
Every **10th level** spawns a boss with a simple movement + fire pattern:
- 10, 20, 30, …, 100

See `Assets/components/Gameplay/Enemies/BossController.cs`.

## Difficulty Scaling
Difficulty scales by level index:
- Level length increases
- Enemy density increases
- Hazard density increases
- Breakable blocks become more common later

See `Assets/levels/LevelRuntime.cs` (`GenerateLevel`).

## Example Layout Patterns (What the generator produces)
- Flat run + small platforms early (Jungle)
- Wider platform gaps and more spikes mid-game (Desert/Ice)
- More traps + more enemies late-game (Lava/Underwater)
- Hidden strawberry: placed in an upper “hidden” pocket behind a faint wall

## Custom Levels (future-ready)
If you want hand-authored levels later, the recommended approach is:
- Add `ScriptableObject` level assets under `Assets/levels/`
- Store an array of platform/pickup/enemy spawn records
- Switch `LevelRuntime.GenerateLevel` to load the asset when present, otherwise fall back to procedural generation

