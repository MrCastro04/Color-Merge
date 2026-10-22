using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts.Interfaces
{
    public interface IDragContext
    {
        Canvas Canvas { get; }
        RectTransform CanvasRt { get; }
    }
}
