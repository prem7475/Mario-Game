using System.Collections.Generic;
using MarioGame.Levels;
using UnityEngine;

namespace MarioGame.Components.Audio
{
    public enum SfxId
    {
        Jump,
        Coin,
        Powerup,
        EnemyHit,
        GameOver
    }

    public sealed class AudioService : MonoBehaviour
    {
        private static AudioService _instance;
        private readonly Dictionary<SfxId, AudioClip> _sfx = new();
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        public bool MusicEnabled { get; private set; } = true;
        public bool SfxEnabled { get; private set; } = true;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.loop = true;
            _musicSource.volume = 0.45f;

            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.loop = false;
            _sfxSource.playOnAwake = false;
            _sfxSource.volume = 0.8f;

            // Try to load real assets from Resources (offline-friendly), otherwise fall back to silent placeholders.
            _sfx[SfxId.Jump] = LoadOrSilent("Audio/SFX/jump", "jump_silent");
            _sfx[SfxId.Coin] = LoadOrSilent("Audio/SFX/coin", "coin_silent");
            _sfx[SfxId.Powerup] = LoadOrSilent("Audio/SFX/powerup", "powerup_silent");
            _sfx[SfxId.EnemyHit] = LoadOrSilent("Audio/SFX/enemyhit", "enemyhit_silent");
            _sfx[SfxId.GameOver] = LoadOrSilent("Audio/SFX/gameover", "gameover_silent");

            // Default global music fallback.
            var music = Resources.Load<AudioClip>("Audio/Music/bgm");
            if (music != null) SetMusic(music);
        }

        private static AudioClip LoadOrSilent(string resourcesPath, string fallbackName)
        {
            var clip = Resources.Load<AudioClip>(resourcesPath);
            return clip != null ? clip : AudioClip.Create(fallbackName, 4410, 1, 44100, false);
        }

        public static void PlaySfx(SfxId id)
        {
            if (_instance == null || !_instance.SfxEnabled)
                return;

            if (_instance._sfx.TryGetValue(id, out var clip) && clip != null)
                _instance._sfxSource.PlayOneShot(clip);
        }

        public static void SetSoundEnabled(bool enabled)
        {
            if (_instance == null)
                return;

            _instance.ApplyMusicEnabled(enabled);
            _instance.ApplySfxEnabled(enabled);
        }

        public static void SetMusicForWorld(WorldThemeId world)
        {
            if (_instance == null)
                return;

            var db = Resources.Load<AudioThemeDatabase>("Audio/AudioThemeDatabase");
            var music = db != null ? db.GetMusic(world) : null;
            if (music == null)
                music = Resources.Load<AudioClip>("Audio/Music/bgm");

            if (music != null)
                _instance.SetMusic(music);
        }

        public static void SetMusicEnabled(bool enabled)
        {
            if (_instance == null) return;
            _instance.ApplyMusicEnabled(enabled);
        }

        public static void SetSfxEnabled(bool enabled)
        {
            if (_instance == null) return;
            _instance.ApplySfxEnabled(enabled);
        }

        private void SetMusic(AudioClip clip)
        {
            if (_musicSource.clip == clip)
                return;

            _musicSource.clip = clip;
            if (MusicEnabled)
                _musicSource.Play();
        }

        private void ApplyMusicEnabled(bool enabled)
        {
            MusicEnabled = enabled;
            _musicSource.mute = !enabled;
            if (enabled && _musicSource.clip != null && !_musicSource.isPlaying)
                _musicSource.Play();
        }

        private void ApplySfxEnabled(bool enabled)
        {
            SfxEnabled = enabled;
            _sfxSource.mute = !enabled;
        }
    }
}
