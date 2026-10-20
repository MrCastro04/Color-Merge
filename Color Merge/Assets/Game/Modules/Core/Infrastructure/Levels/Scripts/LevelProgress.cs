using Assets.Game.Modules.Core.Save.Scripts;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Какой уровень запускать. Хранится в ISaveService — игра продолжается с того же уровня между сессиями.
    public class LevelProgress
    {
        private const string CURRENT_LEVEL_KEY = "current_level_index";

        private readonly ISaveService _saveService;

        public int CurrentLevelIndex => _saveService.GetInt(CURRENT_LEVEL_KEY);

        public LevelProgress(ISaveService saveService)
        {
            _saveService = saveService;
        }

        public void SelectLevel(int levelIndex)
        {
            _saveService.SetInt(CURRENT_LEVEL_KEY, levelIndex);
            _saveService.Save();
        }
    }
}
