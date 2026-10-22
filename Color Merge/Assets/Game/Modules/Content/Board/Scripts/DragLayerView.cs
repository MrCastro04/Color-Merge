using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using UnityEngine;

namespace Assets.Game.Modules.Content.Board.Scripts
{
    [RequireComponent(typeof(Canvas))]
    public class DragLayerView : MonoBehaviour, IDragContext
    {
        public Canvas Canvas => GetComponent<Canvas>();
        public RectTransform CanvasRt => (RectTransform)transform;
    }
}
