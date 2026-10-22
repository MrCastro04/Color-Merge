using System.Threading;
using Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts;
using Assets.Game.Modules.Core.Infrastructure.MidLoader.Scripts;
using Assets.Game.Modules.Core.SceneManagement.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    public class LevelLoader
    {
        private const float CLOSE_FADE = 0.3f;
        private const float MENU_HOLD_AFTER_LOAD = 1f;
        private const float LEVEL_HOLD_AFTER_LOAD = 0.3f;

        private readonly ISceneLoader _sceneLoader;
        private readonly LoadingTarget _loadingTarget;
        private readonly ScreenFader _screenFader;
        private readonly LifetimeScope _applicationScope;

        public string CurrentLevelScene { get; private set; }
        public bool IsLoading { get; private set; }

        public LevelLoader(
            ISceneLoader sceneLoader, LoadingTarget loadingTarget,
            ScreenFader screenFader, LifetimeScope applicationScope)
        {
            _sceneLoader = sceneLoader;
            _loadingTarget = loadingTarget;
            _screenFader = screenFader;
            _applicationScope = applicationScope;
        }

        public async UniTask LoadAsync(string sceneName, CancellationToken cancellationToken = default)
        {
            if (IsLoading)
                return;

            IsLoading = true;

            bool isMenu = sceneName == SCENE_NAMES.MAIN_MENU;

            if (!isMenu)
                CurrentLevelScene = sceneName;

            _loadingTarget.Set(sceneName,
                showTips: isMenu,
                holdAfterLoad: isMenu ? MENU_HOLD_AFTER_LOAD : LEVEL_HOLD_AFTER_LOAD);

            await _screenFader.FadeInAsync(CLOSE_FADE, cancellationToken);

            using (LifetimeScope.EnqueueParent(_applicationScope))
            {
                await _sceneLoader.LoadAsync(SCENE_NAMES.MID_LOADER, cancellationToken);
            }
        }

        public UniTask RestartAsync(CancellationToken cancellationToken = default)
        {
            return LoadAsync(CurrentLevelScene, cancellationToken);
        }

        public async UniTask ActivateAsync(PreloadedScene scene)
        {
            using (LifetimeScope.EnqueueParent(_applicationScope))
            {
                await scene.ActivateAsync();
            }

            IsLoading = false;
        }
    }
}
