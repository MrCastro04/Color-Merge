using System.Threading;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
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

        // Конфетти победы и анимации доски одновременно держат сотни твинов — ёмкость задаём сразу,
        // чтобы DOTween не расширял её на лету с предупреждениями.
        private const int TWEENS_CAPACITY = 500;
        private const int SEQUENCES_CAPACITY = 150;

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            DOTween.SetTweensCapacity(TWEENS_CAPACITY, SEQUENCES_CAPACITY);

            // Одноразовые звуки грузятся один раз на всю игру — дальше играют без задержки.
            await _sfxOneShotPlayer.WarmUpAsync(cancellation);

            await _levelLoader.LoadAsync(SCENE_NAMES.MAIN_MENU, cancellation);
        }
    }
}
