using System.Threading;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_11
{
    public class Level_11_Controller : IAsyncStartable
    {
        private readonly LevelGameplay _levelGameplay;

        public Level_11_Controller(LevelGameplay levelGameplay)
        {
            _levelGameplay = levelGameplay;
        }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            return _levelGameplay.RunAsync(cancellation);
        }
    }
}
