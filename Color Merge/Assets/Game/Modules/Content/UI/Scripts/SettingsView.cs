using System;
using Assets.Game.Modules.Core.UI.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Окно настроек: громкость музыки и звуков. Сама громкость меняется снаружи (контроллер → ISoundService).
    // В меню окно закрывается крестиком; на уровне внизу кнопки MENU и RESUME (крестика нет).
    // Необязательные кнопки можно не назначать.
    public class SettingsView : UIScreen
    {
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _menuButton;

        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;
        public event Action OnCloseClicked;
        public event Action OnMenuClicked;

        protected override void Awake()
        {
            base.Awake();

            _musicSlider.onValueChanged.AddListener(value => OnMusicVolumeChanged?.Invoke(value));
            _sfxSlider.onValueChanged.AddListener(value => OnSfxVolumeChanged?.Invoke(value));

            if (_closeButton != null)
                _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

            if (_menuButton != null)
                _menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }

        // Без вызова событий — только показать текущие значения.
        public void SetValues(float musicVolume, float sfxVolume)
        {
            _musicSlider.SetValueWithoutNotify(musicVolume);
            _sfxSlider.SetValueWithoutNotify(sfxVolume);
        }

        public void SetButtonsInteractable(bool isInteractable)
        {
            if (_cg == null)
                _cg = GetComponent<CanvasGroup>();

            _cg.interactable = isInteractable;
        }
    }
}
