using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts.Interfaces
{
    public interface IItemFactory
    {
        UniTask WarmUpAsync(CancellationToken cancellationToken = default);
        ItemController Create(ItemConfig config, Transform parent);
        void Destroy(ItemController itemController);
    }
}
