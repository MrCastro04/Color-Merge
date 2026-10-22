using System;
using Assets.Game.Modules.Core.Timer.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    public class LevelButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private GameObject _lockIcon;
        [SerializeField] private Image _background;
        [SerializeField] private Color _unlockedColor = Color.white;
        [SerializeField] private Color _lockedColor = new(0.6f, 0.6f, 0.65f, 1f);

        private bool _isListening;

        public int Index { get; private set; }
        public bool IsLocked { get; private set; }

        public event Action<LevelButtonView> OnClicked;

        public void Setup(int index, LevelButtonState state)
        {
            Index = index;
            IsLocked = state.IsLocked;

            _numberText.text = index.ToString();
            _numberText.gameObject.SetActive(!state.IsLocked);
            _lockIcon.SetActive(state.IsLocked);
            _background.color = state.IsLocked ? _lockedColor : _unlockedColor;

            bool showBestTime = !state.IsLocked && state.HasBestTime;
            _bestTimeText.gameObject.SetActive(showBestTime);

            if (showBestTime)
                _bestTimeText.text = TimeFormatter.Format(state.BestTime);

            if (!_isListening)
            {
                _button.onClick.AddListener(() => OnClicked?.Invoke(this));
                _isListening = true;
            }
        }

        public void PlayLockedShake()
        {
            transform.DOKill(true);
            transform.DOShakeRotation(0.35f, new Vector3(0, 0, 12f), 12, 0f).SetLink(gameObject);
        }
    }
}
