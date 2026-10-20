namespace Assets.Game.Modules.Core.Save.Scripts
{
    public interface ISaveService
    {
        bool HasKey(string key);

        int GetInt(string key, int defaultValue = 0);
        void SetInt(string key, int value);

        float GetFloat(string key, float defaultValue = 0f);
        void SetFloat(string key, float value);

        void DeleteKey(string key);
        void Save();
    }
}
