using System;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Сценарий уровня (как Tutorial_N_Controller в Combo-Dungeon):
    // загрузка конфига → сборка доски → игра → победа / поражение.
    public class LevelController : IAsyncStartable, IDisposable
    {
        private readonly IAssetProvider _assetProvider;
        private readonly LevelLoader _levelLoader;
        private readonly LevelConfigReference _levelConfigReference;
        private readonly IItemFactory _itemFactory;
        private readonly ISlotFactory _slotFactory;
        private readonly BoardController _boardController;
        private readonly Generator _generator;
        private readonly LevelTimer _levelTimer;
        private readonly BestTimeRepository _bestTimeRepository;
        private readonly HudView _hudView;
        private readonly WinScreen _winScreen;

        private LevelConfig _levelConfig;
        private CancellationToken _cancellationToken;
        private bool _isFinished;

        public LevelController(
            IAssetProvider assetProvider, LevelLoader levelLoader,
            LevelConfigReference levelConfigReference,
            IItemFactory itemFactory, ISlotFactory slotFactory,
            BoardController boardController, Generator generator,
            LevelTimer levelTimer, BestTimeRepository bestTimeRepository,
            HudView hudView, WinScreen winScreen)
        {
            _assetProvider = assetProvider;
            _levelLoader = levelLoader;
            _levelConfigReference = levelConfigReference;
            _itemFactory = itemFactory;
            _slotFactory = slotFactory;
            _boardController = boardController;
            _generator = generator;
            _levelTimer = levelTimer;
            _bestTimeRepository = bestTimeRepository;
            _hudView = hudView;
            _winScreen = winScreen;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _cancellationToken = cancellation;
            _winScreen.HideImmediate();

            await LoadAsync(cancellation);

            BuildLevel();

            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private async UniTask LoadAsync(CancellationToken cancellation)
        {
            _levelConfig = await _assetProvider.LoadAsync<LevelConfig>(_levelConfigReference, cancellation);

            await UniTask.WhenAll(
                _itemFactory.WarmUpAsync(cancellation),
                _slotFactory.WarmUpAsync(cancellation));
        }

        private void BuildLevel()
        {
            _boardController.Build(_levelConfig);
            _generator.Setup(_levelConfig.ItemChain.First, _levelConfig.GeneratorCharges);

            _levelTimer.Reset();
            _hudView.SetLevelName(_levelConfig.name);

            bool hasBestTime = _bestTimeRepository.TryGet(_levelConfig.ID, out var bestTime);
            _hudView.SetBestTime(hasBestTime, bestTime);
        }

        private void Subscribe()
        {
            _boardController.OnItemMerged += HandleOnItemMerged;
            _boardController.OnGoalItemCreated += HandleOnGoalItemCreated;
            _generator.OnChargesEnded += HandleOnChargesEnded;
            _levelTimer.OnTimeChanged += _hudView.SetTime;
            _winScreen.OnRestartClicked += HandleOnRestartClicked;
            _winScreen.OnNextClicked += HandleOnNextClicked;
        }

        private void Unsubscribe()
        {
            _boardController.OnItemMerged -= HandleOnItemMerged;
            _boardController.OnGoalItemCreated -= HandleOnGoalItemCreated;
            _generator.OnChargesEnded -= HandleOnChargesEnded;
            _levelTimer.OnTimeChanged -= _hudView.SetTime;
            _winScreen.OnRestartClicked -= HandleOnRestartClicked;
            _winScreen.OnNextClicked -= HandleOnNextClicked;
        }

        private void HandleOnItemMerged(ItemController item)
        {
            if (!_levelTimer.IsRunning && !_isFinished)
                _levelTimer.Start();
        }

        private void HandleOnGoalItemCreated(ItemController goalItem)
        {
            WinAsync(goalItem).Forget();
        }

        private void HandleOnChargesEnded()
        {
            // TODO: поражение — генератор пуст и на доске нет пар для мерджа (_boardController.HasAvailableMerge()).
            // Проверять после каждого мерджа, показывать экран поражения с рестартом.
        }

        private async UniTaskVoid WinAsync(ItemController goalItem)
        {
            if (_isFinished)
                return;

            _isFinished = true;

            _levelTimer.Stop();
            _boardController.SetInteractable(false);
            _generator.SetInteractable(false);

            float time = _levelTimer.Elapsed;
            bool isNewRecord = _bestTimeRepository.TrySave(_levelConfig.ID, time);
            _bestTimeRepository.TryGet(_levelConfig.ID, out var bestTime);

            await goalItem.View.PlayWinAsync(_cancellationToken);

            _winScreen.SetResult(time, bestTime, isNewRecord, _levelLoader.HasNextLevel);

            await _winScreen.ShowAsync(_cancellationToken);
        }

        private void HandleOnRestartClicked()
        {
            _levelLoader.RestartAsync().Forget();
        }

        private void HandleOnNextClicked()
        {
            _levelLoader.LoadNextAsync().Forget();
        }
    }
}
