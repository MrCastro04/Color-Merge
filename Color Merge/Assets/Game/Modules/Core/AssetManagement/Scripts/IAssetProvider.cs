using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Core.AssetManagement.Scripts
{
    public interface IAssetProvider
    {
        UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellationToken = default)
            where T : UnityEngine.Object;

        UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken = default)
            where T : UnityEngine.Object;

        void Release(AssetReference reference);
        void Release(string key);
        void ReleaseAll();
    }
}
