using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Game.Modules.Core.SceneManagement.Scripts
{
    // В отличие от Combo-Dungeon здесь нет LifetimeScope.EnqueueParent: Core не знает про VContainer.
    // Родителя скоупа сцене выставляет LevelLoader (Infrastructure) на время загрузки / активации.
    public class SceneLoader : ISceneLoader
    {
        // При allowSceneActivation = false Unity останавливает progress на 0.9 — это и есть «загружено».
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
