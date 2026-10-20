using UnityEngine;

namespace Assets.Game.Modules.Core.Save.Scripts
{
    public class PlayerPrefsSaveService : ISaveService
    {
        public bool HasKey(string key) => PlayerPrefs.HasKey(key);

        public int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);
        public void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);

        public float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);
        public void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);

        public void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public void Save() => PlayerPrefs.Save();
    }
}
