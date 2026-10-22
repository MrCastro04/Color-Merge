using System.Threading;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts
{
    public class BootController : IAsyncStartable
    {
        private const int TWEENS_CAPACITY = 500;
        private const int SEQUENCES_CAPACITY = 150;
        
        private readonly LevelLoader _levelLoader;
        private readonly SFXOneShotPlayer _sfxOneShotPlayer;

        public BootController(LevelLoader levelLoader, SFXOneShotPlayer sfxOneShotPlayer)
        {
            _levelLoader = levelLoader;
            _sfxOneShotPlayer = sfxOneShotPlayer;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            Application.targetFrameRate = 60;
            
            DOTween.SetTweensCapacity(TWEENS_CAPACITY, SEQUENCES_CAPACITY);

            await _sfxOneShotPlayer.WarmUpAsync(cancellation);

            await _levelLoader.LoadAsync(SCENE_NAMES.MAIN_MENU, cancellation);
        }
    }
}
