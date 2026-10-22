using System;
using Assets.Game.Modules.Core.Utils.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    // Кнопка генератора: иконка предмета, который он сейчас создаёт, «Lvl N» сверху справа, заряды.
    // На туторе поднимается над затемнением (IChangeSortGroupInCanvas) и пульсирует (IPulseObject).
    public class GeneratorButtonView : MonoBehaviour, IChangeSortGroupInCanvas, IPulseObject
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _chargesText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Image _icon;
        [SerializeField] private Canvas _sortingCanvas;
        [SerializeField] private float _maxPulseScale = 1.12f;
        [SerializeField] private float _pulseDuration = 0.45f;

        private float _sceneX;
        private bool _isPositionCached;

        public event Action OnClicked;

        public Button Button => _button;

        public string DesiredGroup => SORTING_LAYERS.TUTOR;
        public string StartGroup { get; set; } = SORTING_LAYERS.DEFAULT;
        public Canvas SortingCanvas => _sortingCanvas;

        public Sequence PulseSequence { get; set; }
        public RectTransform RT => (RectTransform)transform;
        public Ease PulseEase => Ease.InOutSine;
        public float MaxPulseScale => _maxPulseScale;
        public float MinPulseScale => 1f;
        public float PulseDuration => _pulseDuration;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleOnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnClick);
        }

        // charges < 0 — без ограничения, счётчик скрыт
        public void SetCharges(int charges)
        {
            if (_chargesText == null)
                return;

            _chargesText.gameObject.SetActive(charges >= 0);
            _chargesText.text = charges.ToString();
        }

        public void SetLevel(int level)
        {
            if (_levelText != null)
                _levelText.text = $"Lvl {level}";
        }

        public void SetIcon(Sprite sprite)
        {
            if (_icon != null)
                _icon.sprite = sprite;
        }

        // Без кнопки улучшения генератор стоит по центру, с ней — на своём месте из сцены.
        // shiftX — сдвиг вправо, когда слева есть кнопка конфетной бомбы.
        public void SetCentered(bool isCentered, float shiftX = 0f)
        {
            var rt = (RectTransform)transform;

            if (!_isPositionCached)
            {
                _sceneX = rt.anchoredPosition.x;
                _isPositionCached = true;
            }

            rt.anchoredPosition = new Vector2((isCentered ? 0f : _sceneX) + shiftX, rt.anchoredPosition.y);
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        public void PlayUpgrade()
        {
            if (_icon == null)
                return;

            _icon.transform.DOKill(true);
            _icon.transform.DOPunchScale(Vector3.one * 0.35f, 0.45f, 6, 0.5f).SetLink(gameObject);
        }

        private void HandleOnClick()
        {
            transform.DOKill(true);
            transform.DOPunchScale(Vector3.one * -0.1f, 0.2f, 6, 0.5f).SetLink(gameObject);

            OnClicked?.Invoke();
        }
    }
}
