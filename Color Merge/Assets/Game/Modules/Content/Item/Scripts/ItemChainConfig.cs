using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    [CreateAssetMenu(menuName = "Game/Item/Item Chain Config", fileName = "ItemChain")]
    public class ItemChainConfig : ScriptableObject
    {
        private const int MIN_CHAIN_LENGTH = 6;
        private const int MAX_CHAIN_LENGTH = 8;

        [SerializeField] private List<ItemConfig> _items = new();

        public IReadOnlyList<ItemConfig> Items => _items;
        public ItemConfig First => _items[0];
        public ItemConfig Last => _items[_items.Count - 1];

        public int GetLevel(ItemConfig config) => _items.IndexOf(config) + 1;

        public bool TryGetNext(ItemConfig config, out ItemConfig next)
        {
            int index = _items.IndexOf(config);

            if (index < 0 || index >= _items.Count - 1)
            {
                next = null;
                return false;
            }

            next = _items[index + 1];
            return true;
        }

        public ItemConfig GetByLevel(int level) => _items[level - 1];

        private void OnValidate()
        {
            if (_items.Count < MIN_CHAIN_LENGTH || _items.Count > MAX_CHAIN_LENGTH)
                Debug.LogWarning($"{name}: у ланцюжку {_items.Count} предметів, потрібно {MIN_CHAIN_LENGTH}–{MAX_CHAIN_LENGTH}.", this);
        }
    }
}
