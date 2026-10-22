using System;
using Assets.Game.Modules.Core.Utils.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    // Кнопка улучшения генератора (стрелка вверх). Активна, когда хватает энергии:
    // сверху «Lvl N -> Lvl N+1», иначе — сколько энергии накоплено.
    // На туторе поднимается над затемнением (IChangeSortGroupInCanvas).
    public class UpgradeButtonView : MonoBehaviour, IChangeSortGroupInCanvas
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _labelText;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField, Range(0f, 1f)] private float _inactiveAlpha = 0.5f;
        [SerializeField] private Canvas _sortingCanvas;

        private Tween _readyPulse;

        public event Action OnClicked;

        public RectTransform RT => (RectTransform)transform;
        // Подпись над кнопкой (энергия «0/3» или «Lvl N -> Lvl N+1») — на неё указывает стрелка тутора.
        public RectTransform LabelRT => (RectTransform)_labelText.transform;

        public string DesiredGroup => SORTING_LAYERS.TUTOR;
        public string StartGroup { get; set; } = SORTING_LAYERS.DEFAULT;
        public Canvas SortingCanvas => _sortingCanvas;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleOnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnClick);
            StopPulse();
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetReady(int currentLevel, int nextLevel)
        {
            _labelText.text = $"Lvl {currentLevel} -> Lvl {nextLevel}";
            SetActive(true);
        }

        public void SetNotReady(int energy, int energyNeeded)
        {
            _labelText.text = $"{energy}/{energyNeeded}";
            SetActive(false);
        }

        public void SetMaxed()
        {
            _labelText.text = "MAX";
            SetActive(false);
        }

        private void SetActive(bool isActive)
        {
            bool wasActive = _button.interactable;

            _button.interactable = isActive;
            _canvasGroup.alpha = isActive ? 1f : _inactiveAlpha;

            if (isActive && !wasActive)
                StartPulse();
            else if (!isActive)
                StopPulse();
        }

        private void StartPulse()
        {
            StopPulse();
            _readyPulse = transform
                .DOScale(1.06f, 0.6f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void StopPulse()
        {
            _readyPulse?.Kill();
            _readyPulse = null;
            transform.localScale = Vector3.one;
        }

        private void HandleOnClick() => OnClicked?.Invoke();
    }
}
