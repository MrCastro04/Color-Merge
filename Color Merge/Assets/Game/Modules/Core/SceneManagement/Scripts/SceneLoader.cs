using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Game.Modules.Core.SceneManagement.Scripts
{
    public class SceneLoader : ISceneLoader
    {
        private const float READY_PROGRESS = 0.9f;

        public async UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default)
        {
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask(cancellationToken: cancellationToken);
        }

        public async UniTask<PreloadedScene> PreloadAsync(string sceneName, IProgress<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (operation.progress < READY_PROGRESS)
            {
                progress?.Report(operation.progress / READY_PROGRESS);
                await UniTask.Yield(cancellationToken);
            }

            progress?.Report(1f);

            return new PreloadedScene(sceneName, operation);
        }
    }
}
