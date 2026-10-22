using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    public class MainMenuView : MonoBehaviour
    {
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private float _playPulseScale = 1.06f;

        private Tween _playPulse;

        public event Action OnPlayClicked;
        public event Action OnQuitClicked;

        private void OnEnable()
        {
            _playButton.onClick.AddListener(HandleOnPlayClicked);

            if (_quitButton != null)
                _quitButton.onClick.AddListener(HandleOnQuitClicked);

            _playPulse = _playButton.transform
                .DOScale(_playPulseScale, 0.8f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void OnDisable()
        {
            _playButton.onClick.RemoveListener(HandleOnPlayClicked);

            if (_quitButton != null)
                _quitButton.onClick.RemoveListener(HandleOnQuitClicked);

            _playPulse?.Kill();
            _playButton.transform.localScale = Vector3.one;
        }

        public void SetInteractable(bool isInteractable)
        {
            _playButton.interactable = isInteractable;

            if (_quitButton != null)
                _quitButton.interactable = isInteractable;
        }

        private void HandleOnPlayClicked() => OnPlayClicked?.Invoke();
        private void HandleOnQuitClicked() => OnQuitClicked?.Invoke();
    }
}
