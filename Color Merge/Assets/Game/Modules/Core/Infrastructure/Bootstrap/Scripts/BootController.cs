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
            // TODO: экран выбора уровня (LevelSelectView) → _levelLoader.LoadAsync(SCENE_NAMES.LEVEL_N)
            await _levelLoader.LoadAsync(SCENE_NAMES.LEVEL_1, cancellation);
        }
    }
}
