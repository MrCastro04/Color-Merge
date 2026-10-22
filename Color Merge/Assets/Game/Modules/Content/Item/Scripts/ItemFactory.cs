using System.Threading;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    public class ItemFactory : IItemFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly ItemAssets _itemAssets;
        private readonly IObjectResolver _resolver;

        private ItemView _prefab;

        public ItemFactory(IAssetProvider assetProvider, ItemAssets itemAssets, IObjectResolver resolver)
        {
            _assetProvider = assetProvider;
            _itemAssets = itemAssets;
            _resolver = resolver;
        }

        public async UniTask WarmUpAsync(CancellationToken cancellationToken = default)
        {
            if (_prefab.IsNotNull())
                return;

            var prefab = await _assetProvider.LoadAsync<GameObject>(_itemAssets.ItemPrefab, cancellationToken);
            _prefab = prefab.GetComponent<ItemView>();
        }

        public ItemController Create(ItemConfig config, Transform parent)
        {
            var view = _resolver.Instantiate(_prefab, parent);

            return new ItemController(config, view);
        }

        public void Destroy(ItemController itemController)
        {
            if (itemController.IsNull() || itemController.View.IsNull())
                return;

            Object.Destroy(itemController.View.gameObject);
        }
    }
}
