using System.Threading;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Slot.Scripts
{
    public class SlotFactory : ISlotFactory
    {
        private readonly IAssetProvider _assetProvider;
        private readonly SlotAssets _slotAssets;
        private readonly IObjectResolver _resolver;

        private SlotView _prefab;

        public SlotFactory(IAssetProvider assetProvider, SlotAssets slotAssets, IObjectResolver resolver)
        {
            _assetProvider = assetProvider;
            _slotAssets = slotAssets;
            _resolver = resolver;
        }

        public async UniTask WarmUpAsync(CancellationToken cancellationToken = default)
        {
            if (_prefab.IsNotNull())
                return;

            var prefab = await _assetProvider.LoadAsync<GameObject>(_slotAssets.SlotPrefab, cancellationToken);
            _prefab = prefab.GetComponent<SlotView>();
        }

        // Как HandController в Combo-Dungeon: у каждого слота свой контроллер, создаётся руками.
        public SlotController Create(int id, Transform parent, ISlotDropHandler dropHandler)
        {
            var view = _resolver.Instantiate(_prefab, parent);
            view.name = $"Slot {id}";
            view.SetID(id);

            var controller = new SlotController(view, dropHandler);
            controller.Initialize();

            return controller;
        }

        public void Destroy(SlotController slotController)
        {
            if (slotController.IsNull())
                return;

            slotController.Dispose();

            if (slotController.View.IsNotNull())
                Object.Destroy(slotController.View.gameObject);
        }
    }
}
