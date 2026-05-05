# Mario-Style 2D Mobile Platformer (Unity)

Unity-based 2D side-scrolling platformer scaffold targeting **Android + iOS**, designed to run **fully offline** and store progress locally.

## Requirements
- Unity **2022.3 LTS** (recommended; newer versions should auto-upgrade)
- Android Build Support (for APK/AAB)
- iOS Build Support (Mac required to produce an IPA)

## Open the Project
Open this folder in Unity Hub as a Unity project (it contains `Assets/`, `Packages/`, `ProjectSettings/`).

## Play (Editor)
- Create/open any scene (even an empty one) and press **Play**.
- The game bootstraps itself via `RuntimeInitializeOnLoadMethod` and spawns the level + player + UI automatically.

## Mobile Controls
The scene includes on-screen buttons (Left/Right/Jump) wired into the input layer used by the player controller.

## Sound (Offline)
Audio loads from Unity `Resources` (no streaming/network):
- Music: `Assets/assets/Audio/Resources/Audio/Music/bgm.*`
- SFX: `Assets/assets/Audio/Resources/Audio/SFX/jump.*`, `coin.*`, `powerup.*`, `gameover.*`

## Offline Save / Progress
Progress is stored locally via a single JSON save file under `Application.persistentDataPath`.

## Build Android (APK/AAB)
1. Unity: **File -> Build Settings...**
2. Select **Android**, click **Switch Platform**
3. **Player Settings...**
   - Package name: e.g. `com.yourname.marioplatformer`
   - Scripting backend: IL2CPP (recommended)
4. **Build** (AAB recommended for Play Store). For a local APK, enable **Build App Bundle (Google Play)** = off.

## Build iOS (Xcode)
1. Unity: **File -> Build Settings...**
2. Select **iOS**, click **Switch Platform**
3. **Build** -> outputs an Xcode project
4. Open in Xcode on macOS -> set signing team -> build/run/archive

## Project Structure (Unity Assets)
- `Assets/assets/` art, audio, materials
- `Assets/components/` gameplay scripts (player, enemies, pickups)
- `Assets/scenes/` scenes
- `Assets/physics/` physics helpers (ground checks, layers)
- `Assets/utils/` save system, helpers
- `Assets/levels/` level data + generation
