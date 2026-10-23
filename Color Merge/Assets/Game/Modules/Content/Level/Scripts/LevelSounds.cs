using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.GUI.Popups.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Level.Scripts.Conditions;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    public class LevelSounds : IInitializable, IDisposable
    {
        private readonly SFXOneShotPlayer _sfx;
        private readonly BoardController _boardController;
        private readonly Generator.Scripts.GeneratorController _generatorController;
        private readonly WinScreen _winScreen;
        private readonly LoseScreen _loseScreen;
        private readonly Generator.Scripts.UpgradeGeneratorSystem _upgradeGeneratorSystem;
        private readonly CandyBomb.Scripts.CandyBombSystem _candyBombSystem;
        private readonly LevelConditionsChecker _conditionsChecker;

        public LevelSounds(
            SFXOneShotPlayer sfx, BoardController boardController,
            Generator.Scripts.GeneratorController generatorController, WinScreen winScreen, LoseScreen loseScreen,
            Generator.Scripts.UpgradeGeneratorSystem upgradeGeneratorSystem, LevelConditionsChecker conditionsChecker,
            CandyBomb.Scripts.CandyBombSystem candyBombSystem)
        {
            _sfx = sfx;
            _boardController = boardController;
            _generatorController = generatorController;
            _winScreen = winScreen;
            _loseScreen = loseScreen;
            _upgradeGeneratorSystem = upgradeGeneratorSystem;
            _candyBombSystem = candyBombSystem;
            _conditionsChecker = conditionsChecker;
        }

        public void Initialize()
        {
            _boardController.OnItemPicked += HandleOnItemPicked;
            _boardController.OnItemMoved += HandleOnItemMoved;
            _boardController.OnItemReturned += HandleOnItemReturned;
            _boardController.OnItemsSwapped += HandleOnItemsSwapped;
            _boardController.OnItemSpawned += HandleOnItemSpawned;
            _boardController.OnItemMerged += HandleOnItemMerged;
            _conditionsChecker.OnAllConditionsCompleted += HandleOnAllConditionsCompleted;
            _generatorController.OnSpawnFailed += HandleOnSpawnFailed;
            _upgradeGeneratorSystem.OnUpgraded += HandleOnGeneratorUpgraded;
            _candyBombSystem.OnBombCreated += HandleOnBombCreated;
            _candyBombSystem.OnExploded += HandleOnBombExploded;
            _winScreen.OnRestartClicked += HandleOnButtonClicked;
            _winScreen.OnMenuClicked += HandleOnButtonClicked;
            _winScreen.OnContinueClicked += HandleOnButtonClicked;
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
            _conditionsChecker.OnAllConditionsCompleted -= HandleOnAllConditionsCompleted;
            _generatorController.OnSpawnFailed -= HandleOnSpawnFailed;
            _upgradeGeneratorSystem.OnUpgraded -= HandleOnGeneratorUpgraded;
            _candyBombSystem.OnBombCreated -= HandleOnBombCreated;
            _candyBombSystem.OnExploded -= HandleOnBombExploded;
            _winScreen.OnRestartClicked -= HandleOnButtonClicked;
            _winScreen.OnMenuClicked -= HandleOnButtonClicked;
            _winScreen.OnContinueClicked -= HandleOnButtonClicked;
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
        private void HandleOnAllConditionsCompleted() => _sfx.Play(SFXOneShotType.Win);
        private void HandleOnSpawnFailed() => _sfx.Play(SFXOneShotType.Return);
        private void HandleOnButtonClicked() => _sfx.Play(SFXOneShotType.ButtonClick);
        private void HandleOnGeneratorUpgraded() => _sfx.Play(SFXOneShotType.Merge);
        private void HandleOnBombCreated() => _sfx.Play(SFXOneShotType.Spawn);
        private void HandleOnBombExploded() => _sfx.Play(SFXOneShotType.Win);
        private void HandleOnStarShown(int starIndex) => _sfx.Play(SFXOneShotType.Spawn);
    }
}
