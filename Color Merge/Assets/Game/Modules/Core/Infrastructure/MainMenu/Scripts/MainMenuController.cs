using System;
using System.Threading;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Sound.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MainMenu.Scripts
{
    // Сценарий главного меню: шторка уходит, музыка меню, «Играть» → окно выбора уровня → уровень через Mid Loader,
    // окно настроек с громкостью.
    public class MainMenuController : IAsyncStartable, IDisposable
    {
        private const float SHOW_MENU_FADE = 0.3f;

        // Порядок уровней в окне выбора. Пока открыт только первый.
        private static readonly string[] LEVEL_SCENES = { SCENE_NAMES.LEVEL_1, SCENE_NAMES.LEVEL_2, SCENE_NAMES.LEVEL_3 };
        private const int UNLOCKED_LEVELS = 1;

        private readonly MainMenuView _mainMenuView;
        private readonly SettingsView _settingsView;
        private readonly LevelSelectView _levelSelectView;
        private readonly LevelLoader _levelLoader;
        private readonly ScreenFader _screenFader;
        private readonly MusicPlayer _musicPlayer;
        private readonly SFXOneShotPlayer _sfx;
        private readonly ISoundService _soundService;

        private CancellationToken _cancellationToken;

        public MainMenuController(
            MainMenuView mainMenuView, SettingsView settingsView, LevelSelectView levelSelectView,
            LevelLoader levelLoader, ScreenFader screenFader, MusicPlayer musicPlayer,
            SFXOneShotPlayer sfx, ISoundService soundService)
        {
            _mainMenuView = mainMenuView;
            _settingsView = settingsView;
            _levelSelectView = levelSelectView;
            _levelLoader = levelLoader;
            _screenFader = screenFader;
            _musicPlayer = musicPlayer;
            _sfx = sfx;
            _soundService = soundService;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _cancellationToken = cancellation;

            _settingsView.HideImmediate();
            _levelSelectView.HideImmediate();
            _levelSelectView.Setup(UNLOCKED_LEVELS);

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
            _mainMenuView.OnSettingsClicked += HandleOnSettingsClicked;
            _settingsView.OnCloseClicked += HandleOnSettingsCloseClicked;
            _settingsView.OnMusicVolumeChanged += _soundService.SetMusicVolume;
            _settingsView.OnSfxVolumeChanged += _soundService.SetSfxVolume;
            _levelSelectView.OnLevelSelected += HandleOnLevelSelected;
            _levelSelectView.OnLockedLevelClicked += HandleOnLockedLevelClicked;
            _levelSelectView.OnCloseClicked += HandleOnLevelSelectCloseClicked;
        }

        private void Unsubscribe()
        {
            _mainMenuView.OnPlayClicked -= HandleOnPlayClicked;
            _mainMenuView.OnSettingsClicked -= HandleOnSettingsClicked;
            _settingsView.OnCloseClicked -= HandleOnSettingsCloseClicked;
            _settingsView.OnMusicVolumeChanged -= _soundService.SetMusicVolume;
            _settingsView.OnSfxVolumeChanged -= _soundService.SetSfxVolume;
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

            _levelLoader.LoadAsync(LEVEL_SCENES[levelIndex]).Forget();
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

        private void HandleOnSettingsClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _settingsView.SetValues(_soundService.MusicVolume, _soundService.SfxVolume);
            _settingsView.ShowAsync(_cancellationToken).Forget();
        }

        private void HandleOnSettingsCloseClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _settingsView.HideAsync(_cancellationToken).Forget();
        }
    }
}
