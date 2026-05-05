using System;
using System.IO;
using UnityEngine;

namespace MarioGame.Utils.Save
{
    public sealed class SaveService
    {
        private const string FileName = "save.json";
        private const string PlayerPrefsKey = "mariogame.save.json";
        private SaveData _data;

        public SaveService()
        {
            _data = Load();
        }

        public SaveData Data => _data;

        public void Save()
        {
            var json = JsonUtility.ToJson(_data, prettyPrint: true);
            // PlayerPrefs copy (requested for mobile-friendly local saves).
            PlayerPrefs.SetString(PlayerPrefsKey, json);
            PlayerPrefs.Save();

            var path = GetPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? Application.persistentDataPath);
            File.WriteAllText(path, json);
        }

        private static SaveData Load()
        {
            try
            {
                if (PlayerPrefs.HasKey(PlayerPrefsKey))
                {
                    var prefsJson = PlayerPrefs.GetString(PlayerPrefsKey);
                    var prefsData = JsonUtility.FromJson<SaveData>(prefsJson);
                    if (prefsData != null)
                        return prefsData;
                }

                var path = GetPath();
                if (!File.Exists(path))
                    return new SaveData();

                var fileJson = File.ReadAllText(path);
                var fileData = JsonUtility.FromJson<SaveData>(fileJson);
                return fileData ?? new SaveData();
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Failed to load save data. Starting fresh. {ex.Message}");
                return new SaveData();
            }
        }

        private static string GetPath()
        {
            return Path.Combine(Application.persistentDataPath, FileName);
        }
    }
}
