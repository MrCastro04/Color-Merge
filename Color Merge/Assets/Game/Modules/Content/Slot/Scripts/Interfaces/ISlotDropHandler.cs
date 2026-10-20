using Assets.Game.Modules.Content.Item.Scripts;

namespace Assets.Game.Modules.Content.Slot.Scripts.Interfaces
{
    public interface ISlotDropHandler
    {
        // Синхронный: принять дроп (ItemView.ConfirmDrop) нужно до OnEndDrag.
        void HandleDrop(SlotController targetSlot, ItemView itemView);
    }
}
