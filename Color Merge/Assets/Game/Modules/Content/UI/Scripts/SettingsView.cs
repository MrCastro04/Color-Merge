using System;
using Assets.Game.Modules.Core.UI.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    public class SettingsView : UIScreen
    {
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Toggle _vibrationToggle;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _menuButton;
        [Tooltip("Лише в головному меню: стерти весь прогрес")]
        [SerializeField] private Button _clearProgressButton;

        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;
        public event Action<bool> OnVibrationChanged;
        public event Action OnCloseClicked;
        public event Action OnMenuClicked;
        public event Action OnClearProgressClicked;

        protected override void Awake()
        {
            base.Awake();

            _musicSlider.onValueChanged.AddListener(value => OnMusicVolumeChanged?.Invoke(value));
            _sfxSlider.onValueChanged.AddListener(value => OnSfxVolumeChanged?.Invoke(value));

            if (_vibrationToggle != null)
                _vibrationToggle.onValueChanged.AddListener(isOn => OnVibrationChanged?.Invoke(isOn));

            if (_closeButton != null)
                _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

            if (_resumeButton != null)
                _resumeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());

            if (_menuButton != null)
                _menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());

            if (_clearProgressButton != null)
                _clearProgressButton.onClick.AddListener(() => OnClearProgressClicked?.Invoke());
        }

        public void SetValues(float musicVolume, float sfxVolume, bool isVibrationEnabled)
        {
            _musicSlider.SetValueWithoutNotify(musicVolume);
            _sfxSlider.SetValueWithoutNotify(sfxVolume);

            if (_vibrationToggle != null)
                _vibrationToggle.SetIsOnWithoutNotify(isVibrationEnabled);
        }

        public void SetButtonsInteractable(bool isInteractable)
        {
            if (_cg == null)
                _cg = GetComponent<CanvasGroup>();

            _cg.interactable = isInteractable;
        }
    }
}
