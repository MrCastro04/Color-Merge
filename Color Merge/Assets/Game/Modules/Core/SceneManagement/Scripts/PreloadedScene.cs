using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Core.SceneManagement.Scripts
{
    public class PreloadedScene
    {
        private readonly AsyncOperation _operation;

        public string SceneName { get; }

        public PreloadedScene(string sceneName, AsyncOperation operation)
        {
            SceneName = sceneName;
            _operation = operation;
        }

        public async UniTask ActivateAsync(CancellationToken cancellationToken = default)
        {
            _operation.allowSceneActivation = true;
            await _operation.ToUniTask(cancellationToken: cancellationToken);
        }
    }
}
