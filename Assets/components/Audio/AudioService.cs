using System.Collections.Generic;
using UnityEngine;

namespace MarioGame.Components.Audio
{
    public enum SfxId
    {
        Jump,
        Coin,
        Powerup,
        GameOver
    }

    public sealed class AudioService : MonoBehaviour
    {
        private static AudioService _instance;
        private readonly Dictionary<SfxId, AudioClip> _sfx = new();
        private AudioSource _musicSource;
        private AudioSource _sfxSource;

        public bool SoundEnabled { get; private set; } = true;

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
            _sfx[SfxId.GameOver] = LoadOrSilent("Audio/SFX/gameover", "gameover_silent");

            var music = Resources.Load<AudioClip>("Audio/Music/bgm");
            if (music != null)
            {
                _musicSource.clip = music;
                _musicSource.Play();
            }
        }

        private static AudioClip LoadOrSilent(string resourcesPath, string fallbackName)
        {
            var clip = Resources.Load<AudioClip>(resourcesPath);
            return clip != null ? clip : AudioClip.Create(fallbackName, 4410, 1, 44100, false);
        }

        public static void PlaySfx(SfxId id)
        {
            if (_instance == null || !_instance.SoundEnabled)
                return;

            if (_instance._sfx.TryGetValue(id, out var clip) && clip != null)
                _instance._sfxSource.PlayOneShot(clip);
        }

        public static void SetSoundEnabled(bool enabled)
        {
            if (_instance == null)
                return;

            _instance.SoundEnabled = enabled;
            _instance._musicSource.mute = !enabled;
            _instance._sfxSource.mute = !enabled;
        }
    }
}
