using System;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.CandyBomb.Scripts
{
    // Кнопка конфетной бомбы (слева от генератора): сверху молния и «энергия/цена».
    // Хватает энергии — кнопка активна; нажали — на месте кнопки появляется бомба (в _holder),
    // её можно перетащить на поле; пока бомба ждёт, кнопка пульсирует.
    public class CandyBombButtonView : MonoBehaviour, IChangeSortGroupInCanvas
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _energyText;
        [SerializeField] private HolderView _holder;
        [SerializeField] private Canvas _sortingCanvas;
        [SerializeField, Range(0f, 1f)] private float _inactiveAlpha = 0.45f;

        private Tween _readyPulse;

        public event Action OnClicked;

        public RectTransform RT => (RectTransform)transform;
        public RectTransform HolderRT => (RectTransform)_holder.transform;

        public string DesiredGroup => SORTING_LAYERS.TUTOR;
        public string StartGroup { get; set; } = SORTING_LAYERS.DEFAULT;
        public Canvas SortingCanvas => _sortingCanvas;

        private void Awake()
        {
            _holder.Init();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleOnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnClick);
            SetReady(false);
        }

        public void SetVisible(bool isVisible)
        {
            gameObject.SetActive(isVisible);
        }

        public void SetEnergy(int energy, int cost)
        {
            _energyText.text = $"{Mathf.Min(energy, cost)}/{cost}";
        }

        // Можно ли нажать: хватает энергии и бомба ещё не ждёт на кнопке.
        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
            _icon.color = new Color(1f, 1f, 1f, isInteractable ? 1f : _inactiveAlpha);
        }

        public void PlaceBomb(ItemView bombView)
        {
            _holder.Set(bombView.RT, out _);
            bombView.PlaySpawnAsync().Forget();
        }

        // Бомба ждёт на кнопке — кнопка мягко пульсирует, иконка под бомбой скрыта.
        public void SetReady(bool isReady)
        {
            _readyPulse?.Kill();
            _readyPulse = null;
            transform.localScale = Vector3.one;
            _icon.enabled = !isReady;

            if (!isReady)
                return;

            _readyPulse = transform
                .DOScale(1.08f, 0.55f)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void HandleOnClick() => OnClicked?.Invoke();
    }
}
