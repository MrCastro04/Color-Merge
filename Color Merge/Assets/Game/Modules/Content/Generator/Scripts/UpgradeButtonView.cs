using System;
using Assets.Game.Modules.Core.Utils.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    public class UpgradeButtonView : MonoBehaviour, IChangeSortGroupInCanvas
    {
        private const float ENERGY_ICON_GAP = 8f;

        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _labelText;
        [Tooltip("Блискавка ліворуч від «енергія/ціна»")]
        [SerializeField] private RectTransform _energyIcon;
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField, Range(0f, 1f)] private float _inactiveAlpha = 0.5f;
        [SerializeField] private Canvas _sortingCanvas;

        private Tween _readyPulse;
        private float _sceneX;
        private bool _isPositionCached;

        public event Action OnClicked;

        public RectTransform RT => (RectTransform)transform;
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

        public void SetShift(float shiftX)
        {
            var rt = (RectTransform)transform;

            if (!_isPositionCached)
            {
                _sceneX = rt.anchoredPosition.x;
                _isPositionCached = true;
            }

            rt.anchoredPosition = new Vector2(_sceneX + shiftX, rt.anchoredPosition.y);
        }

        public void SetReady(int currentLevel, int nextLevel)
        {
            _labelText.text = $"Lvl {currentLevel} -> Lvl {nextLevel}";
            SetEnergyIconVisible(false);
            SetActive(true);
        }

        public void SetNotReady(int energy, int energyNeeded)
        {
            _labelText.text = $"{energy}/{energyNeeded}";
            SetEnergyIconVisible(true);
            SetActive(false);
        }

        public void SetMaxed()
        {
            _labelText.text = "MAX";
            SetEnergyIconVisible(false);
            SetActive(false);
        }

        private void SetEnergyIconVisible(bool isVisible)
        {
            if (_energyIcon == null)
                return;

            _energyIcon.gameObject.SetActive(isVisible);

            if (!isVisible)
                return;

            _labelText.ForceMeshUpdate();
            float halfWidth = _labelText.preferredWidth * 0.5f;
            _energyIcon.anchoredPosition = new Vector2(-halfWidth - ENERGY_ICON_GAP, _energyIcon.anchoredPosition.y);
        }

        private void SetActive(bool isActive)
        {
            bool wasActive = _button.interactable;

            _button.interactable = isActive;

            if (isActive)
                _canvasGroup.alpha = 1f;
            else
                _canvasGroup.alpha = _inactiveAlpha;

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
