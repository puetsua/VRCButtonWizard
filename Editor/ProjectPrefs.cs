using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Puetsua.VRCButtonWizard.Editor
{
    [InitializeOnLoad]
    internal static class ProjectPrefs
    {
        private const string ToolId = "VRCButtonWizard";
        private static string PrefFilePath => Path.Combine(Application.dataPath, $"project_pref_{ToolId}.json");
        private static readonly PrefData Prefs;

        static ProjectPrefs()
        {
            Prefs = LoadDataStorage();
        }

        public static string GetString(string key)
        {
            return Prefs.GetValue(key);
        }

        public static void SetString(string key, string value)
        {
            Prefs.SetKey(key, value);
            SaveDataStorage();
        }

        public static int GetInt(string key, int defaultValue)
        {
            string value = Prefs.GetValue(key);
            if (int.TryParse(value, out int integer))
            {
                return integer;
            }

            return defaultValue;
        }

        public static void SetInt(string key, int value)
        {
            Prefs.SetKey(key, value.ToString());
            SaveDataStorage();
        }

        public static bool GetBool(string key, bool defaultValue)
        {
            string value = Prefs.GetValue(key);
            if (bool.TryParse(value, out bool boolean))
            {
                return boolean;
            }

            return defaultValue;
        }

        public static void SetBool(string key, bool value)
        {
            Prefs.SetKey(key, value.ToString());
            SaveDataStorage();
        }

        private static PrefData LoadDataStorage()
        {
            if (!File.Exists(PrefFilePath))
            {
                return new PrefData();
            }

            var json = File.ReadAllText(PrefFilePath);
            try
            {
                var data = JsonUtility.FromJson<PrefData>(json);
                return data;
            }
            catch (ArgumentException e)
            {
                Debug.LogWarning($"{e}");
                return new PrefData();
            }
        }

        private static void SaveDataStorage()
        {
            var json = JsonUtility.ToJson(Prefs);
            File.WriteAllText(PrefFilePath, json);
        }

        [Serializable]
        private class PrefData
        {
            public List<KeyValuePair> keyValuePairs = new List<KeyValuePair>();

            public void SetKey(string key, string value)
            {
                var pair = GetPair(key);
                if (pair == null)
                {
                    keyValuePairs.Add(new KeyValuePair
                    {
                        key = key,
                        value = value
                    });
                }
                else
                {
                    pair.value = value;
                }
            }

            public string GetValue(string key)
            {
                var pair = GetPair(key);
                return pair == null ? string.Empty : pair.value;
            }

            public void ClearKey(string key)
            {
                keyValuePairs.RemoveAll(p => p.key == key);
            }

            private KeyValuePair GetPair(string key)
            {
                foreach (var pair in keyValuePairs)
                {
                    if (key == pair.key)
                        return pair;
                }

                return null;
            }
        }

        [Serializable]
        private class KeyValuePair
        {
            public string key;
            public string value;
        }
    }
}