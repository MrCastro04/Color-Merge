using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Game.Modules.Content.Board.Scripts
{
    // Верхний канвас для перетаскиваемого предмета (аналог MaxCanvasView в Combo-Dungeon).
    // Вешается на отдельный Canvas с sortingOrder выше доски.
    [RequireComponent(typeof(Canvas))]
    public class DragLayerView : MonoBehaviour, IDragContext
    {
        public Canvas Canvas => GetComponent<Canvas>();
        public RectTransform CanvasRt => (RectTransform)transform;
    }
}
