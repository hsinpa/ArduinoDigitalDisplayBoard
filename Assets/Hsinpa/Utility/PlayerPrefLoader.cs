using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hsinpa.Pref {
    public class PlayerPrefLoader
    {
        public static T GetJSON<T>(string key) where T : struct
        {
            return GetJSON(key, default(T));
        }

        public static T GetJSON<T>(string key, T fallback) where T : struct
        {
            if (!PlayerPrefs.HasKey(key)) return fallback;

            return JsonUtility.FromJson<T>(PlayerPrefs.GetString(key));
        }

        public static void SaveJSON<T>(string key, T rawJsonData) where T : struct
        {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(rawJsonData));
            PlayerPrefs.Save();
        }

        public static void Delete(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }
    }
}
