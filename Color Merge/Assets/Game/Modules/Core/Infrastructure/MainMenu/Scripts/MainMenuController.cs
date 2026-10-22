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
    // Сценарий главного меню: шторка уходит, музыка меню, «Играть» → окно выбора уровня → уровень через Mid Loader.
    // Окно настроек ведёт SettingsController (общий с уровнем).
    public class MainMenuController : IAsyncStartable, IDisposable
    {
        private const float SHOW_MENU_FADE = 0.3f;

        // Уровни — LEVELS.ALL. Открыт Level 0 и каждый уровень после пройденного предыдущего.

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

        private CancellationToken _cancellationToken;

        public MainMenuController(
            MainMenuView mainMenuView, LevelSelectView levelSelectView, SettingsController settingsController,
            SettingsView settingsView, ISaveService saveService, MessagePopup messagePopup,
            LevelLoader levelLoader, ScreenFader screenFader, MusicPlayer musicPlayer, SFXOneShotPlayer sfx,
            BestTimeRepository bestTimeRepository, LevelProgressRepository levelProgress,
            LockedLevelPopup lockedLevelPopup, TutorialLevelPopup tutorialLevelPopup)
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

        // QUIT — выход из игры (в редакторе — остановка Play Mode).
        private void HandleOnQuitClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            UnityEngine.Application.Quit();
#endif
        }

        // Учебный уровень — сначала окно «новый контент», обычный — сразу загрузка.
        private void HandleOnLevelSelected(int levelIndex)
        {
            _sfx.Play(SFXOneShotType.ButtonClick);

            if (LEVELS.ALL[levelIndex].IsTutorial)
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

            _levelLoader.LoadAsync(LEVELS.ALL[levelIndex].Scene).Forget();
        }

        // Лучшее время показывается только у пройденных неучебных уровней.
        private List<LevelButtonState> GetLevelStates()
        {
            var states = new List<LevelButtonState>(LEVELS.ALL.Length);

            for (int i = 0; i < LEVELS.ALL.Length; i++)
            {
                bool isUnlocked = i == 0 || _levelProgress.IsCompleted(LEVELS.ALL[i - 1].LevelId);
                bool hasBestTime = _bestTimeRepository.TryGet(LEVELS.ALL[i].LevelId, out var bestTime) && !LEVELS.ALL[i].IsTutorial;

                states.Add(new LevelButtonState(isLocked: !isUnlocked, hasBestTime, bestTime));
            }

            return states;
        }

        // Закрытый уровень: подсказка «пройди предыдущий». Уровни нумеруются с нуля → предыдущий = levelIndex - 1.
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

        // Настройки → CLEAR PROGRESS (только в меню): стираем все сохранения, кнопки уровней — заново, окно-подтверждение.
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
