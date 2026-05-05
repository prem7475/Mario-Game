using MarioGame.Levels;
using UnityEngine;

namespace MarioGame.Components.Audio
{
    [CreateAssetMenu(menuName = "MarioGame/Audio/Audio Theme Database", fileName = "AudioThemeDatabase")]
    public sealed class AudioThemeDatabase : ScriptableObject
    {
        [System.Serializable]
        public sealed class WorldAudio
        {
            public WorldThemeId world;
            public AudioClip music;
        }

        public WorldAudio[] worlds;

        public AudioClip GetMusic(WorldThemeId id)
        {
            if (worlds == null) return null;
            for (var i = 0; i < worlds.Length; i++)
            {
                if (worlds[i] != null && worlds[i].world == id)
                    return worlds[i].music;
            }
            return null;
        }
    }
}

