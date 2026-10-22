using Assets.Game.Modules.Content.Item.Scripts;

namespace Assets.Game.Modules.Content.Slot.Scripts.Interfaces
{
    public interface ISlotDropHandler
    {
        void HandleDrop(SlotController targetSlot, ItemView itemView);
    }
}
