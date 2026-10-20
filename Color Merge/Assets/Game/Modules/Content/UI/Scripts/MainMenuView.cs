using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Главное меню: большая кнопка «Играть» (дышит, чтобы притягивать взгляд) и кнопка настроек.
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private float _playPulseScale = 1.06f;

        private Tween _playPulse;

        public event Action OnPlayClicked;
        public event Action OnSettingsClicked;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(HandleOnPlayClicked);
            _settingsButton.onClick.AddListener(HandleOnSettingsClicked);

            _playPulse = _playButton.transform
                .DOScale(_playPulseScale, 0.8f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(HandleOnPlayClicked);
            _settingsButton.onClick.RemoveListener(HandleOnSettingsClicked);

            _playPulse?.Kill();
            _playButton.transform.localScale = Vector3.one;
        }

        public void SetInteractable(bool isInteractable)
        {
            _playButton.interactable = isInteractable;
            _settingsButton.interactable = isInteractable;
        }

        private void HandleOnPlayClicked() => OnPlayClicked?.Invoke();
        private void HandleOnSettingsClicked() => OnSettingsClicked?.Invoke();
    }
}
