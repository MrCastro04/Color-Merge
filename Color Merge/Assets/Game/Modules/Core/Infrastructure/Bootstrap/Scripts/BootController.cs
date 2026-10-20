using System.Threading;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts
{
    public class BootController : IAsyncStartable
    {
        private readonly LevelLoader _levelLoader;
        private readonly SFXOneShotPlayer _sfxOneShotPlayer;

        public BootController(LevelLoader levelLoader, SFXOneShotPlayer sfxOneShotPlayer)
        {
            _levelLoader = levelLoader;
            _sfxOneShotPlayer = sfxOneShotPlayer;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            // Одноразовые звуки грузятся один раз на всю игру — дальше играют без задержки.
            await _sfxOneShotPlayer.WarmUpAsync(cancellation);

            await _levelLoader.LoadAsync(SCENE_NAMES.MAIN_MENU, cancellation);
        }
    }
}
