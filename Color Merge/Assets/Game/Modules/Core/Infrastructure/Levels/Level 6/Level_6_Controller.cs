using System.Threading;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_6
{
    public class Level_6_Controller : IAsyncStartable
    {
        private readonly LevelGameplay _levelGameplay;

        public Level_6_Controller(LevelGameplay levelGameplay)
        {
            _levelGameplay = levelGameplay;
        }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            return _levelGameplay.RunAsync(cancellation);
        }
    }
}
