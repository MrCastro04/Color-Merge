using Assets.Game.Modules.Core.Save.Scripts;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    public class LevelProgressRepository
    {
        private const string KEY_PREFIX = "level_completed_";

        private readonly ISaveService _saveService;

        public LevelProgressRepository(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public bool IsCompleted(string levelId) => _saveService.GetInt(GetKey(levelId)) == 1;

        public void MarkCompleted(string levelId)
        {
            if (IsCompleted(levelId))
                return;

            _saveService.SetInt(GetKey(levelId), 1);
            _saveService.Save();
        }

        private static string GetKey(string levelId) => KEY_PREFIX + levelId;
    }
}
