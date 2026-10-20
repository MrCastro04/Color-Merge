using System;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Core.Sound.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Окно настроек целиком: кнопка-шестерёнка → окно с громкостью → ISoundService.
    // Один и тот же контроллер регистрируется в скоупе меню и в скоупе уровня.
    // Уровень слушает OnOpened/OnClosed (пауза таймера) и OnMenuRequested (выход в меню).
    public class SettingsController : IInitializable, IDisposable
    {
        private readonly SettingsView _settingsView;
        private readonly SettingsButtonView _settingsButtonView;
        private readonly ISoundService _soundService;
        private readonly SFXOneShotPlayer _sfx;

        public bool IsOpened { get; private set; }

        public event Action OnOpened;
        public event Action OnClosed;
        public event Action OnMenuRequested;

        public SettingsController(
            SettingsView settingsView, SettingsButtonView settingsButtonView,
            ISoundService soundService, SFXOneShotPlayer sfx)
        {
            _settingsView = settingsView;
            _settingsButtonView = settingsButtonView;
            _soundService = soundService;
            _sfx = sfx;
        }

        public void Initialize()
        {
            _settingsView.HideImmediate();

            _settingsButtonView.OnClicked += HandleOnSettingsClicked;
            _settingsView.OnCloseClicked += HandleOnCloseClicked;
            _settingsView.OnMenuClicked += HandleOnMenuClicked;
            _settingsView.OnMusicVolumeChanged += _soundService.SetMusicVolume;
            _settingsView.OnSfxVolumeChanged += _soundService.SetSfxVolume;
        }

        public void Dispose()
        {
            _settingsButtonView.OnClicked -= HandleOnSettingsClicked;
            _settingsView.OnCloseClicked -= HandleOnCloseClicked;
            _settingsView.OnMenuClicked -= HandleOnMenuClicked;
            _settingsView.OnMusicVolumeChanged -= _soundService.SetMusicVolume;
            _settingsView.OnSfxVolumeChanged -= _soundService.SetSfxVolume;
        }

        public void SetInteractable(bool isInteractable)
        {
            _settingsButtonView.SetInteractable(isInteractable);
        }

        private void HandleOnSettingsClicked()
        {
            if (IsOpened)
                return;

            IsOpened = true;
            _sfx.Play(SFXOneShotType.ButtonClick);

            _settingsView.SetValues(_soundService.MusicVolume, _soundService.SfxVolume);
            _settingsView.SetButtonsInteractable(true);
            _settingsView.ShowAsync().Forget();

            OnOpened?.Invoke();
        }

        private void HandleOnCloseClicked()
        {
            if (!IsOpened)
                return;

            IsOpened = false;
            _sfx.Play(SFXOneShotType.ButtonClick);
            _settingsView.HideAsync().Forget();

            OnClosed?.Invoke();
        }

        private void HandleOnMenuClicked()
        {
            _sfx.Play(SFXOneShotType.ButtonClick);
            _settingsView.SetButtonsInteractable(false);

            OnMenuRequested?.Invoke();
        }
    }
}
