using System;
using System.Threading;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VContainer;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    // Драг перенесён из CardView (Combo-Dungeon) один в один:
    // на время драга предмет уходит в верхний канвас, слот подтверждает дроп через ConfirmDrop,
    // без подтверждения предмет возвращается в прежний слот.
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(CanvasGroup))]
    [DisallowMultipleComponent]
    public class ItemView : MonoBehaviour, IDraggable, IMouseInteractable
    {
        [SerializeField] protected Image _iconImage;

        [NonSerialized] public bool CanDrag = true;

        protected RectTransform _rt;
        protected CanvasGroup _cg;
        protected int _originalSiblingIndex;

        private RectTransform _returnParent;
        private bool _isDropAccepted;
        private Canvas _dragCanvas;
        private RectTransform _dragCanvasRT;

        public RectTransform RT => _rt;
        public ItemController Controller { get; internal set; }
        public float MaxPointerScale => 1.1f;
        public float MinPointerScale => 1f;

        [Inject]
        public void Construct(IDragContext dragContext)
        {
            _dragCanvas = dragContext.Canvas;
            _dragCanvasRT = dragContext.CanvasRt;
        }

        public void Init()
        {
            _rt = GetComponent<RectTransform>();
            _cg = GetComponent<CanvasGroup>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!CanDrag)
                return;

            _isDropAccepted = false;
            _returnParent = _rt.parent as RectTransform;
            _originalSiblingIndex = _rt.GetSiblingIndex();

            _cg.blocksRaycasts = false;
            _cg.alpha = 0.8f;

            _rt.SetParent(_dragCanvasRT, true);
            _rt.SetAsLastSibling();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!CanDrag)
                return;

            _rt.anchoredPosition += eventData.delta / _dragCanvas.scaleFactor;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!CanDrag)
                return;

            _cg.blocksRaycasts = true;
            _cg.alpha = 1f;

            if (!_isDropAccepted)
                ReturnToPreviousSlot();
        }

        public void ConfirmDrop()
        {
            _isDropAccepted = true;
        }

        public void SetIcon(Sprite sprite)
        {
            _iconImage.sprite = sprite;
        }

        public void SetColor(Color color)
        {
            _iconImage.color = color;
        }

        public void SetInteractable(bool interactable)
        {
            CanDrag = interactable;
            _cg.blocksRaycasts = interactable;
        }

        public UniTask PlaySpawnAsync(CancellationToken cancellationToken = default)
        {
            _rt.localScale = Vector3.zero;

            return _rt
                .DOScale(MinPointerScale, 0.3f)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject)
                .AwaitCompletion(cancellationToken);
        }

        public UniTask PlayMergeAsync(CancellationToken cancellationToken = default)
        {
            _rt.localScale = Vector3.one * 0.6f;

            return DOTween.Sequence()
                .Append(_rt.DOScale(1.3f, 0.15f).SetEase(Ease.OutQuad))
                .Append(_rt.DOScale(MinPointerScale, 0.2f).SetEase(Ease.OutBack))
                .SetLink(gameObject)
                .AwaitCompletion(cancellationToken);
        }

        public UniTask PlayWinAsync(CancellationToken cancellationToken = default)
        {
            return _rt
                .DOPunchScale(Vector3.one * 0.4f, 0.6f, 6, 0.5f)
                .SetLink(gameObject)
                .AwaitCompletion(cancellationToken);
        }

        private void ReturnToPreviousSlot()
        {
            _rt.SetParent(_returnParent, true);
            _rt.SetSiblingIndex(_originalSiblingIndex);

            _rt
                .DOAnchorPos(Vector2.zero, 0.25f)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            GetTweenScale(MaxPointerScale, 0.25f).Play();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GetTweenScale(MinPointerScale, 0.25f).Play();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            GetTweenScale(MinPointerScale - 0.1f, 0.15f).Play();
        }

        private Tween GetTweenScale(float scaleTo, float duration)
        {
            return _rt
                .DOScale(scaleTo, duration)
                .SetEase(Ease.OutBack)
                .SetLink(gameObject);
        }
    }
}
