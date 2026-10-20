using System;
using Assets.Game.Modules.Core.UI.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Окно настроек: громкость музыки и звуков. Сама громкость меняется снаружи (контроллер → ISoundService).
    public class SettingsView : UIScreen
    {
        [SerializeField] private Slider _musicSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private Button _closeButton;

        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;
        public event Action OnCloseClicked;

        protected override void Awake()
        {
            base.Awake();

            _musicSlider.onValueChanged.AddListener(value => OnMusicVolumeChanged?.Invoke(value));
            _sfxSlider.onValueChanged.AddListener(value => OnSfxVolumeChanged?.Invoke(value));
            _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        }

        // Без вызова событий — только показать текущие значения.
        public void SetValues(float musicVolume, float sfxVolume)
        {
            _musicSlider.SetValueWithoutNotify(musicVolume);
            _sfxSlider.SetValueWithoutNotify(sfxVolume);
        }
    }
}
