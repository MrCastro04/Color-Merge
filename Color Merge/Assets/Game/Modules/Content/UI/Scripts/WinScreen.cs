using System;
using System.Threading;
using Assets.Game.Modules.Core.Timer.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Экран победы: время, рекорд, 1–3 звезды (появляются по очереди), конфетти.
    public class WinScreen : UIScreen
    {
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private GameObject _newRecordLabel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;

        [Header("Stars")]
        [SerializeField] private Image[] _stars = Array.Empty<Image>();
        [SerializeField] private Sprite _starOn;
        [SerializeField] private Sprite _starOff;

        [Header("VFX")]
        [SerializeField] private ConfettiView _confetti;

        public event Action OnRestartClicked;
        public event Action OnMenuClicked;
        // Для звука: каждая зажжённая звезда.
        public event Action<int> OnStarShown;

        protected override void Awake()
        {
            base.Awake();

            _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
            _menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }

        public void SetResult(float time, float bestTime, bool isNewRecord)
        {
            _timeText.text = TimeFormatter.Format(time);
            _bestTimeText.text = $"BEST {TimeFormatter.Format(bestTime)}";

            if (_newRecordLabel != null)
                _newRecordLabel.SetActive(isNewRecord);
        }

        public void SetInteractable(bool isInteractable)
        {
            _restartButton.interactable = isInteractable;
            _menuButton.interactable = isInteractable;
        }

        // Окно появилось → конфетти + звёзды по одной.
        public async UniTask ShowWithStarsAsync(int stars, CancellationToken cancellationToken = default)
        {
            foreach (var star in _stars)
            {
                star.sprite = _starOff;
                star.transform.localScale = Vector3.one;
            }

            await ShowAsync(cancellationToken);

            if (_confetti != null)
                _confetti.PlayAsync(cancellationToken).Forget();

            for (int i = 0; i < _stars.Length && i < stars; i++)
            {
                await UniTask.Delay(250, cancellationToken: cancellationToken);

                var star = _stars[i];
                star.sprite = _starOn;
                star.transform.localScale = Vector3.one * 0.3f;
                star.transform
                    .DOScale(1f, 0.35f)
                    .SetEase(Ease.OutBack)
                    .SetLink(star.gameObject);

                OnStarShown?.Invoke(i);
            }
        }
    }
}
