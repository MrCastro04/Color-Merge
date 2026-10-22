using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Assets.Game.Modules.Core.AssetManagement.Scripts
{
    public class AddressablesAssetProvider : IAssetProvider, IDisposable
    {
        private readonly Dictionary<string, AsyncOperationHandle> _handles = new();

        public UniTask<T> LoadAsync<T>(AssetReference reference, CancellationToken cancellationToken = default)
            where T : UnityEngine.Object
        {
            if (reference == null || !reference.RuntimeKeyIsValid())
                throw new ArgumentException($"AssetReference для {typeof(T).Name} не призначено.");

            return LoadAsync<T>(reference.RuntimeKey.ToString(), cancellationToken);
        }

        public async UniTask<T> LoadAsync<T>(string key, CancellationToken cancellationToken = default)
            where T : UnityEngine.Object
        {
            if (_handles.TryGetValue(key, out var cachedHandle))
            {
                if (!cachedHandle.IsDone)
                    await cachedHandle.ToUniTask(cancellationToken: cancellationToken);

                return cachedHandle.Result as T;
            }

            var handle = Addressables.LoadAssetAsync<T>(key);
            _handles[key] = handle;

            return await handle.ToUniTask(cancellationToken: cancellationToken);
        }

        public void Release(AssetReference reference)
        {
            if (reference == null || !reference.RuntimeKeyIsValid())
                return;

            Release(reference.RuntimeKey.ToString());
        }

        public void Release(string key)
        {
            if (!_handles.TryGetValue(key, out var handle))
                return;

            if (handle.IsValid())
                Addressables.Release(handle);

            _handles.Remove(key);
        }

        public void ReleaseAll()
        {
            foreach (var handle in _handles.Values)
            {
                if (handle.IsValid())
                    Addressables.Release(handle);
            }

            _handles.Clear();
        }

        public void Dispose()
        {
            ReleaseAll();
        }
    }
}
