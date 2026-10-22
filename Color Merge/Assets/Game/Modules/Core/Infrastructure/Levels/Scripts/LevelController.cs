using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.CandyBomb.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Level.Scripts.Conditions;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Сценарий уровня (как Tutorial_N_Controller в Combo-Dungeon):
    // загрузка конфига → сборка доски → тутор (если есть) → StartLevelFlow (игра до победы / поражения) → экран результата.
    // Победа — выполнены все условия LevelConfig.Conditions (LevelConditionsChecker.IsAllConditionsCompleted).
    public class LevelController : IAsyncStartable, IDisposable
    {
        private const float SHOW_LEVEL_FADE = 0.4f;
        private const int SHOW_LOSE_DELAY_MS = 400;
        private const float BOMB_BOTTOM_BAR_SHIFT = 90f;

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
        private readonly GeneratorController _generatorController;
        private readonly UpgradeGeneratorSystem _upgradeGeneratorSystem;
        private readonly CandyBombSystem _candyBombSystem;
        private readonly PlayerUpgradesInLevel _upgrades;
        private readonly LevelConditionsChecker _conditionsChecker;
        private readonly LevelTimer _levelTimer;
        private readonly BestTimeRepository _bestTimeRepository;
        private readonly LevelProgressRepository _levelProgress;
        private readonly HudView _hudView;
        private readonly WinScreen _winScreen;
        private readonly LoseScreen _loseScreen;
        private readonly ChainProgressView _chainProgressView;
        private readonly MusicPlayer _musicPlayer;
        private readonly ScreenFader _screenFader;
        private readonly SettingsController _settingsController;
        private readonly ILevelTutorial _levelTutorial;

        private LevelConfig _levelConfig;

        private LevelResult _result;

        // Последний собранный мерджем предмет: если он закрыл условие на предмет — играет анимацию победы.
        private ItemController _lastMergedItem;
        private bool _wasTimerRunningBeforeSettings;

        public LevelController(
            IAssetProvider assetProvider, LevelLoader levelLoader,
            LevelConfigReference levelConfigReference,
            IItemFactory itemFactory, ISlotFactory slotFactory,
            BoardController boardController, GeneratorController generatorController,
            UpgradeGeneratorSystem upgradeGeneratorSystem, CandyBombSystem candyBombSystem, PlayerUpgradesInLevel upgrades,
            LevelConditionsChecker conditionsChecker,
            LevelTimer levelTimer, BestTimeRepository bestTimeRepository, LevelProgressRepository levelProgress,
            HudView hudView, WinScreen winScreen, LoseScreen loseScreen, ChainProgressView chainProgressView,
            MusicPlayer musicPlayer, ScreenFader screenFader, SettingsController settingsController,
            ILevelTutorial levelTutorial)
        {
            _assetProvider = assetProvider;
            _levelLoader = levelLoader;
            _levelConfigReference = levelConfigReference;
            _itemFactory = itemFactory;
            _slotFactory = slotFactory;
            _boardController = boardController;
            _generatorController = generatorController;
            _upgradeGeneratorSystem = upgradeGeneratorSystem;
            _candyBombSystem = candyBombSystem;
            _upgrades = upgrades;
            _conditionsChecker = conditionsChecker;
            _levelTimer = levelTimer;
            _bestTimeRepository = bestTimeRepository;
            _levelProgress = levelProgress;
            _hudView = hudView;
            _winScreen = winScreen;
            _loseScreen = loseScreen;
            _chainProgressView = chainProgressView;
            _musicPlayer = musicPlayer;
            _screenFader = screenFader;
            _settingsController = settingsController;
            _levelTutorial = levelTutorial;
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
                // Тутор — только на учебных уровнях (TUTOR SCENARIO в сцене). Пока идёт — кнопка настроек выключена.
                _settingsController.SetInteractable(false);
                await _levelTutorial.PlayAsync(_levelConfig.ItemChain, cancellation);
                _settingsController.SetInteractable(true);

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

            _result = LevelResult.NONE;
            _boardController.Build(_levelConfig);

            _generatorController.Setup(chain, _levelConfig.GeneratorMaxLevel, _levelConfig.GeneratorCharges);

            _upgrades.Reset();
            _hudView.SetMerges(0, _levelConfig.MergesTarget);
            _upgradeGeneratorSystem.Setup(chain, _levelConfig.IsGeneratorUpgradeEnabled);
            // Есть кнопка конфетной бомбы (слева) — генератор и кнопка улучшения сдвигаются вправо, чтобы влезли все три.
            _candyBombSystem.Setup(_levelConfig);
            float bottomBarShift = _candyBombSystem.IsEnabled ? BOMB_BOTTOM_BAR_SHIFT : 0f;
            _generatorController.SetCentered(!_levelConfig.IsGeneratorUpgradeEnabled, bottomBarShift);
            _upgradeGeneratorSystem.SetShift(bottomBarShift);

            _conditionsChecker.Setup(_levelConfig);

            _levelTimer.Reset();
            _hudView.SetLevelName(_levelConfig.name);
            _hudView.SetTimerVisible(_conditionsChecker.HasTimer);

            if (_conditionsChecker.HasTimer)
            {
                UpdateTimeView(0f);

                // Учебный уровень: таймер есть, рекорда нет.
                bool hasBestTime = _bestTimeRepository.TryGet(_levelConfig.ID, out var bestTime);
                _hudView.SetBestTime(hasBestTime && HasRecord, bestTime);
            }

            SetupChainProgress();
            SetupGoals();
        }

        // Значки условий победы сверху слева — только на обычных (не учебных) уровнях.
        private void SetupGoals()
        {
            var goals = _hudView.Goals;

            if (goals == null)
                return;

            goals.Clear();

            if (_levelConfig.IsTutorial)
                return;

            foreach (var condition in _levelConfig.Conditions)
            {
                if (condition is Get_Item_With_Level_X getItem)
                    goals.AddItemGoal(_levelConfig.ItemChain.GetByLevel(Math.Min(getItem.ItemLevel, _levelConfig.ItemChain.Items.Count)).Icon);
                else if (condition is Upgraded_Generator_With_Level_X generator)
                    goals.AddGeneratorGoal(generator.GeneratorLevel);
            }
        }

        // Игра идёт, пока не выполнены все условия или не наступило поражение.
        // Победу и «нет ходов» выставляют события, лимиты времени (только с Timer) проверяются здесь каждый кадр.
        private async UniTask StartLevelFlow(CancellationToken cancellation)
        {
            while (_result == LevelResult.NONE)
            {
                if (_conditionsChecker.IsTimeUp())
                    _result = LevelResult.LOSE_TIME_UP;

                await UniTask.Yield(cancellation);
            }

            StopGameplay();

            if (_result == LevelResult.WIN)
                await ShowWinAsync(cancellation);
            else
                await ShowLoseAsync(cancellation);
        }

        // Рекорд — только на уровне с таймером и не учебном.
        private bool HasRecord => _conditionsChecker.HasTimer && !_levelConfig.IsTutorial;

        private void StopGameplay()
        {
            _levelTimer.Stop();
            _boardController.SetInteractable(false);
            _generatorController.SetInteractable(false);
            _upgradeGeneratorSystem.SetInteractable(false);
            _candyBombSystem.SetInteractable(false);
        }

        private async UniTask ShowWinAsync(CancellationToken cancellation)
        {
            float time = _levelTimer.Elapsed;

            // Пройден — в меню открывается следующий уровень.
            _levelProgress.MarkCompleted(_levelConfig.ID);

            if (IsWinItem(_lastMergedItem))
                await _lastMergedItem.View.PlayWinAsync(cancellation);

            if (HasRecord)
            {
                bool isNewRecord = _bestTimeRepository.TrySave(_levelConfig.ID, time);
                _bestTimeRepository.TryGet(_levelConfig.ID, out var bestTime);
                _winScreen.SetResult(time, bestTime, isNewRecord);
            }
            else if (_conditionsChecker.HasTimer)
            {
                _winScreen.SetResultWithoutRecord(time);
            }
            else
            {
                _winScreen.SetResultWithoutTime();
            }

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
            _generatorController.OnChargesEnded += CheckOutOfMoves;
            _generatorController.OnSpawnFailed += CheckOutOfMoves;
            _upgrades.OnChanged += HandleOnUpgradesChanged;
            _levelTimer.OnTimeChanged += UpdateTimeView;
            _winScreen.OnRestartClicked += HandleOnRestartClicked;
            _winScreen.OnMenuClicked += HandleOnMenuClicked;
            _winScreen.OnContinueClicked += HandleOnContinueClicked;
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
            _generatorController.OnChargesEnded -= CheckOutOfMoves;
            _generatorController.OnSpawnFailed -= CheckOutOfMoves;
            _upgrades.OnChanged -= HandleOnUpgradesChanged;
            _levelTimer.OnTimeChanged -= UpdateTimeView;
            _winScreen.OnRestartClicked -= HandleOnRestartClicked;
            _winScreen.OnMenuClicked -= HandleOnMenuClicked;
            _winScreen.OnContinueClicked -= HandleOnContinueClicked;
            _loseScreen.OnRestartClicked -= HandleOnRestartClicked;
            _loseScreen.OnMenuClicked -= HandleOnMenuClicked;
            _settingsController.OnOpened -= HandleOnSettingsOpened;
            _settingsController.OnClosed -= HandleOnSettingsClosed;
            _settingsController.OnMenuRequested -= HandleOnMenuClicked;
        }

        // Панель над доской: цепочка до цели-предмета. Прогресс — лучший предмет игрока на поле (PlayerUpgradesInLevel).
        // Показывается только если в условиях есть Get_Item_With_Level_X.
        private void SetupChainProgress()
        {
            var chain = _levelConfig.ItemChain;

            _chainProgressView.gameObject.SetActive(_levelConfig.HasItemGoal);
            var path = new List<ItemConfig>();

            for (int level = 1; level <= _levelConfig.TargetItemLevel; level++)
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

        private bool IsWinItem(ItemController item)
        {
            return item != null && item.View != null && _levelConfig.HasItemGoal
                   && _levelConfig.ItemChain.GetLevel(item.Config) >= _levelConfig.TargetItemLevel;
        }

        // Прогресс вырос (предмет или генератор) — возможно, выполнены все условия.
        private void HandleOnUpgradesChanged()
        {
            _chainProgressView.SetReachedLevel(_upgrades.MaxItemLevel);
            _hudView.SetMerges(_upgrades.MergesCount, _levelConfig.MergesTarget);

            if (_result == LevelResult.NONE && _conditionsChecker.TryCompleteAll())
                _result = LevelResult.WIN;
        }

        private void HandleOnItemMerged(ItemController item)
        {
            _lastMergedItem = item;

            // Таймер (только с условием Timer) стартует с первого мерджа.
            if (_conditionsChecker.HasTimer && !_levelTimer.IsRunning && _result == LevelResult.NONE)
                _levelTimer.Start();

            _upgrades.ReportMerge();
            ReportItem(item);

            CheckOutOfMoves();
        }

        private void HandleOnItemSpawned(ItemController item)
        {
            ReportItem(item);
            CheckOutOfMoves();
        }

        // Нет ходов: пар для мерджа нет, и новый предмет взять негде (генератор пуст или доска заполнена).
        private void CheckOutOfMoves()
        {
            if (_result != LevelResult.NONE || _boardController.HasAvailableMerge())
                return;

            bool canSpawn = _generatorController.HasCharges && _boardController.HasFreeSlot;

            if (!canSpawn)
                _result = LevelResult.LOSE_OUT_OF_MOVES;
        }

        // Есть невыполненное условие с лимитом — обратный отсчёт до ближайшего, иначе секундомер.
        private void UpdateTimeView(float elapsed)
        {
            if (!_conditionsChecker.HasTimer)
                return;

            if (_conditionsChecker.TryGetNearestDeadline(out float remaining))
                _hudView.SetRemainingTime(remaining);
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

        // С экрана победы: следующий уровень по LEVELS.ALL (он уже открыт — этот пройден), с последнего — в меню.
        private void HandleOnContinueClicked()
        {
            _winScreen.SetInteractable(false);

            int next = LEVELS.IndexOfScene(_levelLoader.CurrentLevelScene) + 1;
            bool hasNext = next > 0 && next < LEVELS.ALL.Length;

            _levelLoader.LoadAsync(hasNext ? LEVELS.ALL[next].Scene : SCENE_NAMES.MAIN_MENU).Forget();
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