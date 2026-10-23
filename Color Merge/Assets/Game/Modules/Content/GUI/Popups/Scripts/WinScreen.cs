using System;
using System.Threading;
using Assets.Game.Modules.Core.Timer.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.GUI.Popups.Scripts
{
    public class WinScreen : UIScreen
    {
        private const string WITHOUT_TIME_TEXT = "WELL DONE!";

        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private GameObject _newRecordLabel;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;
        [SerializeField] private Button _continueButton;

        [Header("Stars")]
        [SerializeField] private Image[] _stars = Array.Empty<Image>();
        [SerializeField] private Sprite _starOn;
        [SerializeField] private Sprite _starOff;

        [Header("VFX")]
        [SerializeField] private ConfettiView _confetti;

        public event Action OnRestartClicked;
        public event Action OnMenuClicked;
        public event Action OnContinueClicked;
        public event Action<int> OnStarShown;

        protected override void Awake()
        {
            base.Awake();

            _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
            _menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());

            if (_continueButton != null)
                _continueButton.onClick.AddListener(() => OnContinueClicked?.Invoke());
        }

        public void SetResult(float time, float bestTime, bool isNewRecord)
        {
            _timeText.gameObject.SetActive(true);
            _bestTimeText.gameObject.SetActive(true);

            _timeText.text = TimeFormatter.Format(time);
            _bestTimeText.text = $"BEST {TimeFormatter.Format(bestTime)}";

            if (_newRecordLabel != null)
                _newRecordLabel.SetActive(isNewRecord);
        }

        public void SetResultWithoutRecord(float time)
        {
            _timeText.gameObject.SetActive(true);
            _timeText.text = TimeFormatter.Format(time);
            _bestTimeText.gameObject.SetActive(false);

            if (_newRecordLabel != null)
                _newRecordLabel.SetActive(false);
        }

        public void SetResultWithoutTime()
        {
            _timeText.gameObject.SetActive(true);
            _timeText.text = WITHOUT_TIME_TEXT;
            _bestTimeText.gameObject.SetActive(false);

            if (_newRecordLabel != null)
                _newRecordLabel.SetActive(false);
        }

        public void SetInteractable(bool isInteractable)
        {
            _restartButton.interactable = isInteractable;
            _menuButton.interactable = isInteractable;

            if (_continueButton != null)
                _continueButton.interactable = isInteractable;
        }

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
