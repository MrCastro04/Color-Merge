using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    [CreateAssetMenu(menuName = "Game/Item/Item Assets", fileName = "ItemAssets")]
    public class ItemAssets : ScriptableObject
    {
        [SerializeField] private AssetReferenceGameObject _itemPrefab;

        public AssetReferenceGameObject ItemPrefab => _itemPrefab;
    }
}
