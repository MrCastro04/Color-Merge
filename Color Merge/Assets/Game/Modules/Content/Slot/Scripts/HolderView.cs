using Assets.Game.Modules.Content.Item.Scripts;
using UnityEngine;

namespace Assets.Game.Modules.Content.Slot.Scripts
{
    public class HolderView : MonoBehaviour
    {
        private RectTransform _rectTransform;

        public Vector2 RTPosition => _rectTransform.position;
        public bool InOccupied => GetItemView();

        public void Init()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public ItemView GetItemView()
        {
            return GetComponentInChildren<ItemView>();
        }

        public void Set(RectTransform otherRT, out bool isAdded)
        {
            otherRT.SetParent(_rectTransform, false);
            otherRT.anchoredPosition = Vector2.zero;
            otherRT.localRotation = Quaternion.identity;
            otherRT.localScale = Vector3.one;

            otherRT.sizeDelta = _rectTransform.rect.size;

            isAdded = true;
        }
    }
}
