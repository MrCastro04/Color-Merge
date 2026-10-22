using System;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Slot.Scripts
{
    public class SlotController : IInitializable, IDisposable
    {
        protected readonly SlotView _slotView;
        private readonly ISlotDropHandler _dropHandler;

        public bool IsOccupied => _slotView.IsOccupied;
        public bool IsBlocked => _slotView.IsBlocked;
        public ItemController ItemController => _slotView.ItemView?.Controller;
        public ItemView ItemView => _slotView.ItemView;
        public SlotView View => _slotView;
        public int ID => _slotView.ID;

        public event Action<ItemView> OnItemAttached;

        public SlotController(SlotView slotView, ISlotDropHandler dropHandler)
        {
            _slotView = slotView;
            _dropHandler = dropHandler;
        }

        public virtual void Initialize()
        {
            _slotView.Init();
            _slotView.OnDropRequested += HandleOnDropRequested;
            _slotView.OnItemAttached += HandleOnItemAttached;
        }

        public virtual void Dispose()
        {
            _slotView.OnDropRequested -= HandleOnDropRequested;
            _slotView.OnItemAttached -= HandleOnItemAttached;
        }

        protected void HandleOnDropRequested(ItemView itemView)
        {
            _dropHandler.HandleDrop(this, itemView);
        }

        private void HandleOnItemAttached(ItemView itemView)
        {
            OnItemAttached?.Invoke(itemView);
        }

        public UniTask Attach(ItemView itemView)
        {
            return ConfirmAndAttachToSlot(itemView);
        }

        public void Place(ItemView itemView)
        {
            _slotView.PlaceItem(itemView.RT);
        }

        protected async UniTask ConfirmAndAttachToSlot(ItemView itemView)
        {
            itemView.ConfirmDrop();

            await _slotView.AttachItem(itemView.RT);
        }
    }
}
