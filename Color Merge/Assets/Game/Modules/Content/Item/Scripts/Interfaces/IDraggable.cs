using UnityEngine.EventSystems;

namespace Assets.Game.Modules.Content.Item.Scripts.Interfaces
{
    public interface IDraggable : IBeginDragHandler, IDragHandler, IEndDragHandler
    {
    }
}
