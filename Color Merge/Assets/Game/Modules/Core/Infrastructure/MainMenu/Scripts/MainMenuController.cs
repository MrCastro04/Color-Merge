using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Save.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MainMenu.Scripts
{
    public class MainMenuController : IAsyncStartable, IDisposable
    {
        private const float SHOW_MENU_FADE = 0.3f;

        private readonly MainMenuView _mainMenuView;
        private readonly LevelSelectView _levelSelectView;
        private readonly SettingsController _settingsController;
        private readonly SettingsView _settingsView;
        private readonly ISaveService _saveService;
        private readonly MessagePopup _messagePopup;
        private readonly LevelLoader _levelLoader;
        private readonly ScreenFader _screenFader;
        private readonly MusicPlayer _musicPlayer;
        private readonly SFXOneShotPlayer _sfx;
        private readonly BestTimeRepository _bestTimeRepository;
        private readonly LevelProgressRepository _levelProgress;
        private readonly LockedLevelPopup _lockedLevelPopup;
        private readonly TutorialLevelPopup _tutorialLevelPopup;
        private readonly LevelsConfig _levelsConfig;

        private CancellationToken _cancellationToken;

        public MainMenuController(
            MainMenuView mainMenuView, LevelSelectView levelSelectView, SettingsController settingsController,
            SettingsView settingsView, ISaveService saveService, MessagePopup messagePopup,
            LevelLoader levelLoader, ScreenFader screenFader, MusicPlayer musicPlayer, SFXOneShotPlayer sfx,
            BestTimeRepository bestTimeRepository, LevelProgressRepository levelProgress,
            LockedLevelPopup lockedLevelPopup, TutorialLevelPopup tutorialLevelPopup, LevelsConfig levelsConfig)
        {
            _mainMenuView = mainMenuView;
            _levelSelectView = levelSelectView;
            _settingsController = settingsController;
            _settingsView = settingsView;
            _saveService = saveService;
            _messagePopup = messagePopup;
            _levelLoader = levelLoader;
            _screenFader = screenFader;
            _musicPlayer = musicPlayer;
            _sfx = sfx;
            _bestTimeRepository = bestTimeRepository;
            _levelProgress = levelProgress;
            _lockedLevelPopup = lockedLevelPopup;
            _tutorialLevelPopup = tutorialLevelPopup;
            _levelsConfig = levelsConfig;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _cancellationToken = cancellation;

            _levelSelectView.HideImmediate();
            _levelSelectView.Setup(GetLevelStates());
            _lockedLevelPopup.HideImmediate();
            _messagePopup.HideImmediate();
            _tutorialLevelPopup.HideImmediate();

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
            _mainMenuView.OnQuitClicked += HandleOnQuitClicked;
            _levelSelectView.OnLevelSelected += HandleOnLevelSelected;
            _levelSelectView.OnLockedLevelClicked += HandleOnLockedLevelClicked;
            _levelSelectView.OnCloseClicked += HandleOnLevelSelectCloseClicked;
            _lockedLevelPopup.OnOkClicked += HandleOnLockedPopupOkClicked;
            _settingsView.OnClearProgressClicked += HandleOnClearProgressClicked;
            _messagePopup.OnOkClicked += HandleOnMessagePopupOkClicked;
            _tutorialLevelPopup.OnPlayClicked += HandleOnTutorialPopupPlayClicked;
            _tutorialLevelPopup.OnCloseClicked += HandleOnTutorialPopupCloseClicked;
        }

        private void Unsubscribe()
        {
            _mainMenuView.OnPlayClicked -= HandleOnPlayClicked;
            _mainMenuView.OnQuitClicked -= HandleOnQuitClicked;
            _levelSelectView.OnLevelSelected -= HandleOnLevelSelected;
            _levelSelectView.OnLockedLevelClicked -= HandleOnLockedLevelClicked;
            _levelSelectView.OnCloseClicked -= HandleOnLevelSelectCloseClicked;
            _lockedLevelPopup.OnOkClicked -= HandleOnLockedPopupOkClicked;
            _settingsView.OnClearProgressClicked -= HandleOnClearProgressClicked;
            _messagePopup.OnOkClicked -= HandleOnMessagePopupOkClicked;
            _tutorialLevelPopup.OnPlayClicked -= HandleOnTutorialPopupPlayClicked;
            _tutorialLevelPopup.OnCloseClicked -= HandleOnTutorialPopupCloseClicked;
        }

        private void HandleOnPlayClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _levelSelectView.ShowAsync(_cancellationToken).Forget();
        }

        private void HandleOnQuitClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        private void HandleOnLevelSelected(int levelIndex)
        {
            _sfx.Play(SFXOneShotType.ButtonClick);

            if (_levelsConfig.Get(levelIndex).IsTutorial)
            {
                _tutorialLevelPopup.SetLevel(levelIndex);
                _tutorialLevelPopup.SetInteractable(true);
                _tutorialLevelPopup.ShowAsync(_cancellationToken).Forget();
                return;
            }

            LoadLevel(levelIndex);
        }

        private void HandleOnTutorialPopupPlayClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _tutorialLevelPopup.SetInteractable(false);
            LoadLevel(_tutorialLevelPopup.LevelIndex);
        }

        private void HandleOnTutorialPopupCloseClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _tutorialLevelPopup.HideAsync(_cancellationToken).Forget();
        }

        private void LoadLevel(int levelIndex)
        {
            _levelSelectView.SetInteractable(false);
            _mainMenuView.SetInteractable(false);
            _settingsController.SetInteractable(false);

            _levelLoader.LoadAsync(_levelsConfig.Get(levelIndex).SceneName).Forget();
        }

        private List<LevelButtonState> GetLevelStates()
        {
            var states = new List<LevelButtonState>(_levelsConfig.Count);

            for (int i = 0; i < _levelsConfig.Count; i++)
            {
                var level = _levelsConfig.Get(i);
                bool isUnlocked = i == 0 || _levelProgress.IsCompleted(_levelsConfig.Get(i - 1).Id);
                bool hasBestTime = _bestTimeRepository.TryGet(level.Id, out var bestTime) && !level.IsTutorial;

                states.Add(new LevelButtonState(isLocked: !isUnlocked, hasBestTime, bestTime));
            }

            return states;
        }

        private void HandleOnLockedLevelClicked(int levelIndex)
        {
            _sfx.Play(SFXOneShotType.Return);
            _lockedLevelPopup.SetLevel(levelIndex - 1);
            _lockedLevelPopup.ShowAsync(_cancellationToken).Forget();
        }

        private void HandleOnLockedPopupOkClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _lockedLevelPopup.HideAsync(_cancellationToken).Forget();
        }

        private void HandleOnClearProgressClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);

            _saveService.DeleteAll();
            _saveService.Save();

            _levelSelectView.Setup(GetLevelStates());

            _messagePopup.SetMessage("PROGRESS CLEARED", "All levels and records\nhave been reset.");
            _messagePopup.ShowAsync(_cancellationToken).Forget();
        }

        private void HandleOnMessagePopupOkClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _messagePopup.HideAsync(_cancellationToken).Forget();
        }

        private void HandleOnLevelSelectCloseClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _levelSelectView.HideAsync(_cancellationToken).Forget();
        }
    }
}
