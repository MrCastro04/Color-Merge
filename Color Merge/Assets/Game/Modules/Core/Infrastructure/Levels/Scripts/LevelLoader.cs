using System.Threading;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Core.SceneManagement.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Запуск выбранного уровня (аналог связки SceneLoader + MidLoaderController в Combo-Dungeon).
    // Сцена уровня грузится с EnqueueParent(ApplicationScope) — только так LevelScope получает общие сервисы.
    public class LevelLoader
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LevelProgress _levelProgress;
        private readonly LevelCatalog _levelCatalog;
        private readonly LifetimeScope _applicationScope;

        public int CurrentLevelIndex => _levelProgress.CurrentLevelIndex;
        public bool HasNextLevel => _levelCatalog.HasLevel(CurrentLevelIndex + 1);

        public LevelLoader(
            ISceneLoader sceneLoader, LevelProgress levelProgress,
            LevelCatalog levelCatalog, LifetimeScope applicationScope)
        {
            _sceneLoader = sceneLoader;
            _levelProgress = levelProgress;
            _levelCatalog = levelCatalog;
            _applicationScope = applicationScope;
        }

        public UniTask LoadCurrentAsync(CancellationToken cancellationToken = default)
        {
            if (!_levelCatalog.HasLevel(CurrentLevelIndex))
                _levelProgress.SelectLevel(0);

            return LoadAsync(CurrentLevelIndex, cancellationToken);
        }

        public UniTask LoadLevelAsync(int levelIndex, CancellationToken cancellationToken = default)
        {
            _levelProgress.SelectLevel(levelIndex);

            return LoadAsync(levelIndex, cancellationToken);
        }

        public UniTask LoadNextAsync(CancellationToken cancellationToken = default)
        {
            if (!HasNextLevel)
                return RestartAsync(cancellationToken);

            return LoadLevelAsync(CurrentLevelIndex + 1, cancellationToken);
        }

        public UniTask RestartAsync(CancellationToken cancellationToken = default)
        {
            return LoadAsync(CurrentLevelIndex, cancellationToken);
        }

        private async UniTask LoadAsync(int levelIndex, CancellationToken cancellationToken)
        {
            using (LifetimeScope.EnqueueParent(_applicationScope))
            {
                await _sceneLoader.LoadAsync(_levelCatalog.GetSceneName(levelIndex), cancellationToken);
            }
        }
    }
}
