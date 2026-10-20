using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.SceneManagement.Scripts
{
    public interface ISceneLoader
    {
        UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default);

        // Грузит сцену в фоне, не показывая её. Прогресс 0..1. Показать — PreloadedScene.ActivateAsync().
        UniTask<PreloadedScene> PreloadAsync(string sceneName, IProgress<float> progress = null,
            CancellationToken cancellationToken = default);
    }
}
