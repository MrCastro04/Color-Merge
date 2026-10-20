using System.Threading;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.SceneManagement.Scripts
{
    public interface ISceneLoader
    {
        UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default);
    }
}
