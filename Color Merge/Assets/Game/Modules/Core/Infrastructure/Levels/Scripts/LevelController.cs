using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Сценарий уровня (как Tutorial_N_Controller в Combo-Dungeon):
    // загрузка конфига → сборка доски → StartLevelFlow (игра до победы / поражения) → экран результата.
    public class LevelController : IAsyncStartable, IDisposable
    {
        private const float SHOW_LEVEL_FADE = 0.4f;
        private const int SHOW_LOSE_DELAY_MS = 400;

        private enum LevelResult
        {
            NONE,
            WIN,
            LOSE_OUT_OF_MOVES,
            LOSE_TIME_UP,
        }

        private readonly IAssetProvider _assetProvider;
        private readonly LevelLoader _levelLoader;
        private readonly LevelConfigReference _levelConfigReference;
        private readonly IItemFactory _itemFactory;
        private readonly ISlotFactory _slotFactory;
        private readonly BoardController _boardController;
        private readonly Generator _generator;
        private readonly UpgradeGeneratorSystem _upgradeGeneratorSystem;
        private readonly PlayerUpgradesInLevel _upgrades;
        private readonly LevelTimer _levelTimer;
        private readonly BestTimeRepository _bestTimeRepository;
        private readonly HudView _hudView;
        private readonly WinScreen _winScreen;
        private readonly LoseScreen _loseScreen;
        private readonly ChainProgressView _chainProgressView;
        private readonly MusicPlayer _musicPlayer;
        private readonly ScreenFader _screenFader;
        private readonly SettingsController _settingsController;

        private LevelConfig _levelConfig;
        private LevelResult _result;
        private ItemController _goalItem;
        private bool _wasTimerRunningBeforeSettings;

        public LevelController(
            IAssetProvider assetProvider, LevelLoader levelLoader,
            LevelConfigReference levelConfigReference,
            IItemFactory itemFactory, ISlotFactory slotFactory,
            BoardController boardController, Generator generator,
            UpgradeGeneratorSystem upgradeGeneratorSystem, PlayerUpgradesInLevel upgrades,
            LevelTimer levelTimer, BestTimeRepository bestTimeRepository,
            HudView hudView, WinScreen winScreen, LoseScreen loseScreen, ChainProgressView chainProgressView,
            MusicPlayer musicPlayer, ScreenFader screenFader, SettingsController settingsController)
        {
            _assetProvider = assetProvider;
            _levelLoader = levelLoader;
            _levelConfigReference = levelConfigReference;
            _itemFactory = itemFactory;
            _slotFactory = slotFactory;
            _boardController = boardController;
            _generator = generator;
            _upgradeGeneratorSystem = upgradeGeneratorSystem;
            _upgrades = upgrades;
            _levelTimer = levelTimer;
            _bestTimeRepository = bestTimeRepository;
            _hudView = hudView;
            _winScreen = winScreen;
            _loseScreen = loseScreen;
            _chainProgressView = chainProgressView;
            _musicPlayer = musicPlayer;
            _screenFader = screenFader;
            _settingsController = settingsController;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _winScreen.HideImmediate();
            _loseScreen.HideImmediate();

            await LoadConfigs(cancellation);

            BuildLevel();

            Subscribe();

            _musicPlayer.PlayAsync(MusicType.Gameplay, cancellation).Forget();

            // Доска собрана за шторкой Mid Loader'а — теперь её можно показать.
            await _screenFader.FadeOutAsync(SHOW_LEVEL_FADE, cancellation);

            try
            {
                await StartLevelFlow(cancellation);
            }
            catch (OperationCanceledException)
            {
                // Сцену уровня выгрузили посреди игры (меню, рестарт) — это не ошибка.
            }
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private async UniTask LoadConfigs(CancellationToken cancellation)
        {
            _levelConfig = await _assetProvider.LoadAsync<LevelConfig>(_levelConfigReference, cancellation);

            await UniTask.WhenAll(
                _itemFactory.WarmUpAsync(cancellation),
                _slotFactory.WarmUpAsync(cancellation));
        }

        private void BuildLevel()
        {
            var chain = _levelConfig.ItemChain;

            _boardController.Build(_levelConfig);

            // Генератор не улучшается до цели уровня — иначе выдал бы её сам.
            _generator.Setup(chain, maxLevel: _levelConfig.GoalItemLevel - 1, _levelConfig.GeneratorCharges);

            _upgrades.Reset();
            _upgradeGeneratorSystem.Setup(chain, _levelConfig.IsGeneratorUpgradeEnabled);
            _generator.SetCentered(!_levelConfig.IsGeneratorUpgradeEnabled);

            _levelTimer.Reset();
            _hudView.SetLevelName(_levelConfig.name);
            UpdateTimeView(0f);

            bool hasBestTime = _bestTimeRepository.TryGet(_levelConfig.ID, out var bestTime);
            _hudView.SetBestTime(hasBestTime, bestTime);

            SetupChainProgress();
        }

        // Игра идёт, пока не выполнено условие победы или не наступило поражение.
        // Победу и «нет ходов» выставляют события доски, время (TIMER) проверяется здесь каждый кадр.
        private async UniTask StartLevelFlow(CancellationToken cancellation)
        {
            _result = LevelResult.NONE;

            // TIMER: обратный отсчёт идёт сразу, как только игрок видит доску (не ждёт первого мерджа).
            if (_levelConfig.IsTimerMode)
                _levelTimer.Start();

            while (_result == LevelResult.NONE)
            {
                if (_levelConfig.IsTimerMode && _levelTimer.Elapsed >= _levelConfig.TimeLimit)
                    _result = LevelResult.LOSE_TIME_UP;

                await UniTask.Yield(cancellation);
            }

            StopGameplay();

            if (_result == LevelResult.WIN)
                await ShowWinAsync(cancellation);
            else
                await ShowLoseAsync(cancellation);
        }

        private void StopGameplay()
        {
            _levelTimer.Stop();
            _boardController.SetInteractable(false);
            _generator.SetInteractable(false);
            _upgradeGeneratorSystem.SetInteractable(false);
        }

        private async UniTask ShowWinAsync(CancellationToken cancellation)
        {
            float time = _levelTimer.Elapsed;
            bool isNewRecord = _bestTimeRepository.TrySave(_levelConfig.ID, time);
            _bestTimeRepository.TryGet(_levelConfig.ID, out var bestTime);

            await _goalItem.View.PlayWinAsync(cancellation);

            _winScreen.SetResult(time, bestTime, isNewRecord);

            await _winScreen.ShowWithStarsAsync(_levelConfig.GetStars(time), cancellation);
        }

        private async UniTask ShowLoseAsync(CancellationToken cancellation)
        {
            if (_result == LevelResult.LOSE_TIME_UP)
                _loseScreen.SetReason("TIME'S UP", "You ran out of time.\nTry again!");
            else
                _loseScreen.SetReason("OUT OF MOVES", "No merges left.\nTry again!");

            await UniTask.Delay(SHOW_LOSE_DELAY_MS, cancellationToken: cancellation);
            await _loseScreen.ShowAsync(cancellation);
        }

        private void Subscribe()
        {
            _boardController.OnItemMerged += HandleOnItemMerged;
            _boardController.OnItemSpawned += HandleOnItemSpawned;
            _boardController.OnGoalItemCreated += HandleOnGoalItemCreated;
            _generator.OnChargesEnded += CheckOutOfMoves;
            _generator.OnSpawnFailed += CheckOutOfMoves;
            _upgrades.OnChanged += HandleOnUpgradesChanged;
            _levelTimer.OnTimeChanged += UpdateTimeView;
            _winScreen.OnRestartClicked += HandleOnRestartClicked;
            _winScreen.OnMenuClicked += HandleOnMenuClicked;
            _loseScreen.OnRestartClicked += HandleOnRestartClicked;
            _loseScreen.OnMenuClicked += HandleOnMenuClicked;
            _settingsController.OnOpened += HandleOnSettingsOpened;
            _settingsController.OnClosed += HandleOnSettingsClosed;
            _settingsController.OnMenuRequested += HandleOnMenuClicked;
        }

        private void Unsubscribe()
        {
            _boardController.OnItemMerged -= HandleOnItemMerged;
            _boardController.OnItemSpawned -= HandleOnItemSpawned;
            _boardController.OnGoalItemCreated -= HandleOnGoalItemCreated;
            _generator.OnChargesEnded -= CheckOutOfMoves;
            _generator.OnSpawnFailed -= CheckOutOfMoves;
            _upgrades.OnChanged -= HandleOnUpgradesChanged;
            _levelTimer.OnTimeChanged -= UpdateTimeView;
            _winScreen.OnRestartClicked -= HandleOnRestartClicked;
            _winScreen.OnMenuClicked -= HandleOnMenuClicked;
            _loseScreen.OnRestartClicked -= HandleOnRestartClicked;
            _loseScreen.OnMenuClicked -= HandleOnMenuClicked;
            _settingsController.OnOpened -= HandleOnSettingsOpened;
            _settingsController.OnClosed -= HandleOnSettingsClosed;
            _settingsController.OnMenuRequested -= HandleOnMenuClicked;
        }

        // Панель над доской: цепочка до цели уровня. Прогресс — лучший предмет игрока на поле (PlayerUpgradesInLevel).
        private void SetupChainProgress()
        {
            var chain = _levelConfig.ItemChain;
            var path = new List<ItemConfig>();

            for (int level = 1; level <= _levelConfig.GoalItemLevel; level++)
                path.Add(chain.GetByLevel(level));

            _chainProgressView.Setup(path);

            foreach (var item in _boardController.GetItems())
                ReportItem(item);

            _chainProgressView.SetReachedLevel(_upgrades.MaxItemLevel);
        }

        private void ReportItem(ItemController item)
        {
            _upgrades.ReportItemLevel(_levelConfig.ItemChain.GetLevel(item.Config));
        }

        private void HandleOnUpgradesChanged()
        {
            _chainProgressView.SetReachedLevel(_upgrades.MaxItemLevel);
        }

        private void HandleOnItemMerged(ItemController item)
        {
            ReportItem(item);

            // COLLECT_ITEM: секундомер стартует с первого мерджа.
            if (!_levelTimer.IsRunning && _result == LevelResult.NONE)
                _levelTimer.Start();

            // Собрали предмет-цель — это победа, не поражение.
            if (item.Config != _levelConfig.GoalItem)
                CheckOutOfMoves();
        }

        private void HandleOnItemSpawned(ItemController item)
        {
            ReportItem(item);
            CheckOutOfMoves();
        }

        private void HandleOnGoalItemCreated(ItemController goalItem)
        {
            if (_result != LevelResult.NONE)
                return;

            _goalItem = goalItem;
            _result = LevelResult.WIN;
        }

        // Нет ходов: пар для мерджа нет, и новый предмет взять негде (генератор пуст или доска заполнена).
        private void CheckOutOfMoves()
        {
            if (_result != LevelResult.NONE || _boardController.HasAvailableMerge())
                return;

            bool canSpawn = _generator.HasCharges && _boardController.HasFreeSlot;

            if (!canSpawn)
                _result = LevelResult.LOSE_OUT_OF_MOVES;
        }

        // COLLECT_ITEM — секундомер; TIMER — оставшееся время.
        private void UpdateTimeView(float elapsed)
        {
            if (_levelConfig.IsTimerMode)
                _hudView.SetRemainingTime(Mathf.Max(0f, _levelConfig.TimeLimit - elapsed));
            else
                _hudView.SetTime(elapsed);
        }

        // Пока открыты настройки — пауза: таймер стоит, RESUME продолжает.
        private void HandleOnSettingsOpened()
        {
            _wasTimerRunningBeforeSettings = _levelTimer.IsRunning;
            _levelTimer.Stop();
        }

        private void HandleOnSettingsClosed()
        {
            if (_wasTimerRunningBeforeSettings && _result == LevelResult.NONE)
                _levelTimer.Start();
        }

        // С экрана победы или поражения.
        private void HandleOnRestartClicked()
        {
            _winScreen.SetInteractable(false);
            _loseScreen.SetInteractable(false);
            _levelLoader.RestartAsync().Forget();
        }

        // С экрана победы, поражения или из настроек.
        private void HandleOnMenuClicked()
        {
            _winScreen.SetInteractable(false);
            _loseScreen.SetInteractable(false);
            _settingsController.SetInteractable(false);
            _levelLoader.LoadAsync(SCENE_NAMES.MAIN_MENU).Forget();
        }
    }
}
