using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    // Один предмет цепочки. Уровень предмета = его позиция в ItemChainConfig.
    [CreateAssetMenu(menuName = "Game/Item/Item Config", fileName = "Item_")]
    public class ItemConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private Sprite _icon;
        [SerializeField] private Color _color = Color.white;

        public string ID => _id;
        public Sprite Icon => _icon;
        public Color Color => _color;
    }
}
