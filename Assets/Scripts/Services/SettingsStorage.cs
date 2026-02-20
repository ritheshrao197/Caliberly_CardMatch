using System;
using System.IO;
using UnityEngine;

namespace MemoryGame.Services
{
    /// <summary>
    /// JSON-backed runtime settings/progress storage.
    /// </summary>
    public static class SettingsStorage
    {
        [Serializable]
        private class SettingsData
        {
            public bool sfxEnabled = true;
            public bool musicEnabled = true;
            public int highestLevelIndex = 0;
        }

        private static readonly object Sync = new object();
        private static SettingsData _cache;
        private static string _path;

        public static bool GetSfxEnabled()
        {
            return Load().sfxEnabled;
        }

        public static void SetSfxEnabled(bool enabled)
        {
            var data = Load();
            data.sfxEnabled = enabled;
            Save(data);
        }

        public static bool GetMusicEnabled()
        {
            return Load().musicEnabled;
        }

        public static void SetMusicEnabled(bool enabled)
        {
            var data = Load();
            data.musicEnabled = enabled;
            Save(data);
        }

        public static int GetHighestLevelIndex()
        {
            return Mathf.Max(0, Load().highestLevelIndex);
        }

        public static void SetHighestLevelIndex(int index)
        {
            var data = Load();
            data.highestLevelIndex = Mathf.Max(0, index);
            Save(data);
        }

        public static void ResetProgress()
        {
            var data = Load();
            data.highestLevelIndex = 0;
            Save(data);
        }

        private static SettingsData Load()
        {
            lock (Sync)
            {
                if (_cache != null)
                    return _cache;

                _path = Path.Combine(Application.persistentDataPath, "settings.json");

                if (!File.Exists(_path))
                {
                    _cache = new SettingsData();
                    Save(_cache);
                    return _cache;
                }

                try
                {
                    var json = File.ReadAllText(_path);
                    _cache = JsonUtility.FromJson<SettingsData>(json) ?? new SettingsData();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SettingsStorage] Failed to load settings.json. Using defaults. {ex.Message}");
                    _cache = new SettingsData();
                }

                return _cache;
            }
        }

        private static void Save(SettingsData data)
        {
            lock (Sync)
            {
                _cache = data ?? new SettingsData();
                if (string.IsNullOrEmpty(_path))
                    _path = Path.Combine(Application.persistentDataPath, "settings.json");

                try
                {
                    var folder = Path.GetDirectoryName(_path);
                    if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    var json = JsonUtility.ToJson(_cache, true);
                    File.WriteAllText(_path, json);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"[SettingsStorage] Failed to save settings.json. {ex.Message}");
                }
            }
        }
    }
}
