using System.Threading;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts
{
    public class BootController : IAsyncStartable
    {
        private readonly LevelLoader _levelLoader;

        public BootController(LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            // TODO: экран выбора уровня (LevelSelectView) вместо сразу текущего уровня
            await _levelLoader.LoadCurrentAsync(cancellation);
        }
    }
}
