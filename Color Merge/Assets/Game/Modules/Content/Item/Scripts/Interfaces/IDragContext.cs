using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts.Interfaces
{
    // Верхний канвас, в который предмет переносится на время драга (аналог MaxCanvasView в Combo-Dungeon).
    public interface IDragContext
    {
        Canvas Canvas { get; }
        RectTransform CanvasRt { get; }
    }
}
