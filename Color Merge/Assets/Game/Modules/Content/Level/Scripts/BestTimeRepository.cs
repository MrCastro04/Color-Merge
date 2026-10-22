using Assets.Game.Modules.Core.Save.Scripts;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    public class BestTimeRepository
    {
        private const string KEY_PREFIX = "best_time_";

        private readonly ISaveService _saveService;

        public BestTimeRepository(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public bool TryGet(string levelId, out float bestTime)
        {
            string key = GetKey(levelId);

            if (!_saveService.HasKey(key))
            {
                bestTime = 0f;
                return false;
            }

            bestTime = _saveService.GetFloat(key);
            return true;
        }

        public bool TrySave(string levelId, float time)
        {
            if (TryGet(levelId, out var bestTime) && bestTime <= time)
                return false;

            _saveService.SetFloat(GetKey(levelId), time);
            _saveService.Save();

            return true;
        }

        private static string GetKey(string levelId) => KEY_PREFIX + levelId;
    }
}
