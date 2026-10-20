using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Game.Modules.Core.SceneManagement.Scripts
{
    // В отличие от Combo-Dungeon здесь нет LifetimeScope.EnqueueParent: Core не знает про VContainer.
    // Скоуп сцены находит родителя сам — через поле Parent (parentReference) в инспекторе.
    public class SceneLoader : ISceneLoader
    {
        public async UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default)
        {
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask(cancellationToken: cancellationToken);
        }
    }
}
