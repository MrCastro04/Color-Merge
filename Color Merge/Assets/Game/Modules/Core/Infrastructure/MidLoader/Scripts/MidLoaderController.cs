using System.Threading;
using Assets.Game.Modules.Content.GUI.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.SceneManagement.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MidLoader.Scripts
{
    public class MidLoaderController : IAsyncStartable
    {
        private const float SHOW_LOADER_FADE = 0.4f;
        private const float HIDE_LOADER_FADE = 0.2f;

        private readonly LoadingTarget _loadingTarget;
        private readonly ISceneLoader _sceneLoader;
        private readonly LevelLoader _levelLoader;
        private readonly ScreenFader _screenFader;
        private readonly LoadingBarView _loadingBarView;
        private readonly LoadingTipsView _loadingTipsView;

        public MidLoaderController(
            LoadingTarget loadingTarget, ISceneLoader sceneLoader, LevelLoader levelLoader,
            ScreenFader screenFader, LoadingBarView loadingBarView, LoadingTipsView loadingTipsView)
        {
            _loadingTarget = loadingTarget;
            _sceneLoader = sceneLoader;
            _levelLoader = levelLoader;
            _screenFader = screenFader;
            _loadingBarView = loadingBarView;
            _loadingTipsView = loadingTipsView;
        }

        public async UniTask StartAsync(CancellationToken cancellation = default)
        {
            _loadingBarView.Init();
            _loadingTipsView.SetVisible(_loadingTarget.ShowTips);

            using var tipsCts = CancellationTokenSource.CreateLinkedTokenSource(cancellation);

            if (_loadingTarget.ShowTips)
                _loadingTipsView.PlayAsync(tipsCts.Token).Forget();

            await _screenFader.FadeOutAsync(SHOW_LOADER_FADE, cancellation);

            var preloadedScene = await LoadTargetWithBarAsync(cancellation);

            await UniTask.Delay((int)(_loadingTarget.HoldAfterLoad * 1000), cancellationToken: cancellation);

            tipsCts.Cancel();
            await _screenFader.FadeInAsync(HIDE_LOADER_FADE, cancellation);

            await _levelLoader.ActivateAsync(preloadedScene);
        }

        private async UniTask<PreloadedScene> LoadTargetWithBarAsync(CancellationToken cancellation)
        {
            float loaded = 0f;

            var preloadTask = _sceneLoader
                .PreloadAsync(_loadingTarget.SceneName, Progress.Create<float>(p => loaded = p), cancellation)
                .Preserve();

            while (_loadingBarView.Value < 1f)
            {
                if (loaded > _loadingBarView.Value)
                    await _loadingBarView.AnimateToAsync(loaded, cancellationToken: cancellation);
                else
                    await UniTask.Yield(cancellation);
            }

            return await preloadTask;
        }
    }
}
