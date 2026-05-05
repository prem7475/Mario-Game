using System;
using System.IO;
using UnityEngine;

namespace MarioGame.Utils.Save
{
    public sealed class SaveService
    {
        private const string FileName = "save.json";
        private SaveData _data;

        public SaveService()
        {
            _data = Load();
        }

        public SaveData Data => _data;

        public void Save()
        {
            var json = JsonUtility.ToJson(_data, prettyPrint: true);
            var path = GetPath();
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? Application.persistentDataPath);
            File.WriteAllText(path, json);
        }

        private static SaveData Load()
        {
            try
            {
                var path = GetPath();
                if (!File.Exists(path))
                    return new SaveData();

                var json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<SaveData>(json);
                return data ?? new SaveData();
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

