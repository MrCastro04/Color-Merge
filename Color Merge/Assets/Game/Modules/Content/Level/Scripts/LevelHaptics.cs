using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Core.Vibration.Scripts;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Вибрация уровня (как LevelSounds для звука): короткий отклик телефона на каждый мердж.
    // Выключается галочкой в настройках (IVibrationService.IsEnabled).
    public class LevelHaptics : IInitializable, IDisposable
    {
        private readonly BoardController _boardController;
        private readonly IVibrationService _vibrationService;

        public LevelHaptics(BoardController boardController, IVibrationService vibrationService)
        {
            _boardController = boardController;
            _vibrationService = vibrationService;
        }

        public void Initialize()
        {
            _boardController.OnItemMerged += HandleOnItemMerged;
        }

        public void Dispose()
        {
            _boardController.OnItemMerged -= HandleOnItemMerged;
        }

        private void HandleOnItemMerged(ItemController item) => _vibrationService.VibrateShort();
    }
}
