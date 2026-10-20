using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MainMenu.Scripts
{
    // Сценарий главного меню: шторка уходит, музыка меню, «Играть» → окно выбора уровня → уровень через Mid Loader.
    // Окно настроек ведёт SettingsController (общий с уровнем).
    public class MainMenuController : IAsyncStartable, IDisposable
    {
        private const float SHOW_MENU_FADE = 0.3f;

        // Порядок уровней в окне выбора: сцена + ID рекорда (LevelConfig.ID). Пока открыт только первый.
        private static readonly (string Scene, string RecordId)[] LEVELS =
        {
            (SCENE_NAMES.LEVEL_1, "level_01"),
            (SCENE_NAMES.LEVEL_2, "level_02"),
            (SCENE_NAMES.LEVEL_3, "level_03"),
        };
        private const int UNLOCKED_LEVELS = 1;

        private readonly MainMenuView _mainMenuView;
        private readonly LevelSelectView _levelSelectView;
        private readonly SettingsController _settingsController;
        private readonly LevelLoader _levelLoader;
        private readonly ScreenFader _screenFader;
        private readonly MusicPlayer _musicPlayer;
        private readonly SFXOneShotPlayer _sfx;
        private readonly BestTimeRepository _bestTimeRepository;

        private CancellationToken _cancellationToken;

        public MainMenuController(
            MainMenuView mainMenuView, LevelSelectView levelSelectView, SettingsController settingsController,
            LevelLoader levelLoader, ScreenFader screenFader, MusicPlayer musicPlayer, SFXOneShotPlayer sfx,
            BestTimeRepository bestTimeRepository)
        {
            _mainMenuView = mainMenuView;
            _levelSelectView = levelSelectView;
            _settingsController = settingsController;
            _levelLoader = levelLoader;
            _screenFader = screenFader;
            _musicPlayer = musicPlayer;
            _sfx = sfx;
            _bestTimeRepository = bestTimeRepository;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _cancellationToken = cancellation;

            _levelSelectView.HideImmediate();
            _levelSelectView.Setup(GetLevelStates());

            Subscribe();

            _musicPlayer.PlayAsync(MusicType.MainMenu, cancellation).Forget();

            await _screenFader.FadeOutAsync(SHOW_MENU_FADE, cancellation);
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _mainMenuView.OnPlayClicked += HandleOnPlayClicked;
            _levelSelectView.OnLevelSelected += HandleOnLevelSelected;
            _levelSelectView.OnLockedLevelClicked += HandleOnLockedLevelClicked;
            _levelSelectView.OnCloseClicked += HandleOnLevelSelectCloseClicked;
        }

        private void Unsubscribe()
        {
            _mainMenuView.OnPlayClicked -= HandleOnPlayClicked;
            _levelSelectView.OnLevelSelected -= HandleOnLevelSelected;
            _levelSelectView.OnLockedLevelClicked -= HandleOnLockedLevelClicked;
            _levelSelectView.OnCloseClicked -= HandleOnLevelSelectCloseClicked;
        }

        private void HandleOnPlayClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _levelSelectView.ShowAsync(_cancellationToken).Forget();
        }

        private void HandleOnLevelSelected(int levelIndex)
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _levelSelectView.SetInteractable(false);
            _mainMenuView.SetInteractable(false);
            _settingsController.SetInteractable(false);

            _levelLoader.LoadAsync(LEVELS[levelIndex].Scene).Forget();
        }

        // Лучшее время показывается только у пройденных уровней (рекорд есть только после прохождения).
        private List<LevelButtonState> GetLevelStates()
        {
            var states = new List<LevelButtonState>(LEVELS.Length);

            for (int i = 0; i < LEVELS.Length; i++)
            {
                bool hasBestTime = _bestTimeRepository.TryGet(LEVELS[i].RecordId, out var bestTime);
                states.Add(new LevelButtonState(isLocked: i >= UNLOCKED_LEVELS, hasBestTime, bestTime));
            }

            return states;
        }

        private void HandleOnLockedLevelClicked(int levelIndex)
        {
            _sfx.Play(SFXOneShotType.Return);
        }

        private void HandleOnLevelSelectCloseClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _levelSelectView.HideAsync(_cancellationToken).Forget();
        }
    }
}
