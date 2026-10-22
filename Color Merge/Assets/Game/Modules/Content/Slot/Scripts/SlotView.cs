using System;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Slot.Scripts
{
    public class SlotView : MonoBehaviour, IDropHandler, IWaiter
    {
        [SerializeField] protected int _id = 0;
        [SerializeField] protected Image _backgroundImage;
        [SerializeField] protected Color _blockedColor = new(0.3f, 0.3f, 0.3f, 1f);
        [Tooltip("Стена поверх заблокированной клетки (Square/Dark из GUI-пака)")]
        [SerializeField] protected GameObject _wall;

        protected HolderView _holderView;
        protected bool _isBlocked;
        private Color _backgroundColor;
        private bool _isBackgroundColorCached;

        public event Action<ItemView> OnDropRequested;
        public event Action<ItemView> OnItemAttached;

        public int ID => _id;
        public bool IsBlocked => _isBlocked;
        public bool IsOccupied => _holderView.InOccupied;
        public ItemView ItemView => _holderView.GetItemView();

        public virtual void Init()
        {
            _holderView = GetComponentInChildren<HolderView>(true);
            _holderView.Init();
        }

        public void SetID(int id)
        {
            _id = id;
        }

        public void SetBlocked(bool isBlocked)
        {
            _isBlocked = isBlocked;

            if (_backgroundImage.IsNotNull())
            {
                if (!_isBackgroundColorCached)
                {
                    _backgroundColor = _backgroundImage.color;
                    _isBackgroundColorCached = true;
                }

                if (isBlocked)
                    _backgroundImage.color = _blockedColor;
                else
                    _backgroundImage.color = _backgroundColor;
            }

            if (_wall.IsNotNull())
                _wall.SetActive(isBlocked);
        }

        public virtual void OnDrop(PointerEventData eventData)
        {
            if (_isBlocked) return;
            if (eventData.pointerDrag.IsNull()) return;
            if (!eventData.pointerDrag.TryGetComponent(out ItemView itemView)) return;

            Invoke_OnDropRequested(itemView);
        }

        protected void Invoke_OnDropRequested(ItemView itemView)
        {
            OnDropRequested?.Invoke(itemView);
        }

        public virtual async UniTask AttachItem(RectTransform itemViewRT, float durationToDrag = 0.2f)
        {
            await DragToHolder(itemViewRT, durationToDrag);
        }

        public void PlaceItem(RectTransform itemViewRT)
        {
            _holderView.Set(itemViewRT, out _);

            OnItemAttached?.Invoke(ItemView);
        }

        protected async UniTask DragToHolder(RectTransform itemViewRT, float duration = 0.2f)
        {
            bool isAdded = false;

            itemViewRT
                .DOMove(_holderView.RTPosition, duration)
                .SetEase(Ease.Linear)
                .SetLink(itemViewRT.gameObject)
                .OnComplete(() =>
                {
                    _holderView.Set(itemViewRT, out isAdded);
                });

            await this.WaitUntil(() => isAdded || itemViewRT.IsNull(), destroyCancellationToken);

            OnItemAttached?.Invoke(ItemView);
        }
    }
}
