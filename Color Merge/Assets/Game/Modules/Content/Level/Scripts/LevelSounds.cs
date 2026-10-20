using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Content.UI.Scripts;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Озвучка уровня: слушает события доски, генератора и экрана победы и запускает одноразовые звуки.
    // Сами системы про звук ничего не знают.
    public class LevelSounds : IInitializable, IDisposable
    {
        private readonly SFXOneShotPlayer _sfx;
        private readonly BoardController _boardController;
        private readonly Generator.Scripts.Generator _generator;
        private readonly WinScreen _winScreen;
        private readonly LoseScreen _loseScreen;

        public LevelSounds(
            SFXOneShotPlayer sfx, BoardController boardController,
            Generator.Scripts.Generator generator, WinScreen winScreen, LoseScreen loseScreen)
        {
            _sfx = sfx;
            _boardController = boardController;
            _generator = generator;
            _winScreen = winScreen;
            _loseScreen = loseScreen;
        }

        public void Initialize()
        {
            _boardController.OnItemPicked += HandleOnItemPicked;
            _boardController.OnItemMoved += HandleOnItemMoved;
            _boardController.OnItemReturned += HandleOnItemReturned;
            _boardController.OnItemsSwapped += HandleOnItemsSwapped;
            _boardController.OnItemSpawned += HandleOnItemSpawned;
            _boardController.OnItemMerged += HandleOnItemMerged;
            _boardController.OnGoalItemCreated += HandleOnGoalItemCreated;
            _generator.OnSpawnFailed += HandleOnSpawnFailed;
            _winScreen.OnRestartClicked += HandleOnButtonClicked;
            _winScreen.OnMenuClicked += HandleOnButtonClicked;
            _winScreen.OnStarShown += HandleOnStarShown;
            _loseScreen.OnRestartClicked += HandleOnButtonClicked;
            _loseScreen.OnMenuClicked += HandleOnButtonClicked;
        }

        public void Dispose()
        {
            _boardController.OnItemPicked -= HandleOnItemPicked;
            _boardController.OnItemMoved -= HandleOnItemMoved;
            _boardController.OnItemReturned -= HandleOnItemReturned;
            _boardController.OnItemsSwapped -= HandleOnItemsSwapped;
            _boardController.OnItemSpawned -= HandleOnItemSpawned;
            _boardController.OnItemMerged -= HandleOnItemMerged;
            _boardController.OnGoalItemCreated -= HandleOnGoalItemCreated;
            _generator.OnSpawnFailed -= HandleOnSpawnFailed;
            _winScreen.OnRestartClicked -= HandleOnButtonClicked;
            _winScreen.OnMenuClicked -= HandleOnButtonClicked;
            _winScreen.OnStarShown -= HandleOnStarShown;
            _loseScreen.OnRestartClicked -= HandleOnButtonClicked;
            _loseScreen.OnMenuClicked -= HandleOnButtonClicked;
        }

        private void HandleOnItemPicked(ItemController item) => _sfx.Play(SFXOneShotType.PickUp);
        private void HandleOnItemMoved(ItemController item) => _sfx.Play(SFXOneShotType.Drop);
        private void HandleOnItemReturned(ItemController item) => _sfx.Play(SFXOneShotType.Return);
        private void HandleOnItemsSwapped(ItemController dragged, ItemController target) => _sfx.Play(SFXOneShotType.Swap);
        private void HandleOnItemSpawned(ItemController item) => _sfx.Play(SFXOneShotType.Spawn);
        private void HandleOnItemMerged(ItemController item) => _sfx.Play(SFXOneShotType.Merge);
        private void HandleOnGoalItemCreated(ItemController item) => _sfx.Play(SFXOneShotType.Win);
        private void HandleOnSpawnFailed() => _sfx.Play(SFXOneShotType.Return);
        private void HandleOnButtonClicked() => _sfx.Play(SFXOneShotType.ButtonClick);
        private void HandleOnStarShown(int starIndex) => _sfx.Play(SFXOneShotType.Spawn);
    }
}
