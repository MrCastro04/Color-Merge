using System.Threading;
using Assets.Game.Modules.Core.SceneManagement.Scripts;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Загрузка сцены уровня по имени (аналог SceneLoader.LoadAsync в Combo-Dungeon).
    // Имена — константы SCENE_NAMES.LEVEL_N.
    // Сцена грузится с EnqueueParent(ApplicationScope) — только так LevelScope получает общие сервисы.
    public class LevelLoader
    {
        private readonly ISceneLoader _sceneLoader;
        private readonly LifetimeScope _applicationScope;

        public string CurrentLevelScene { get; private set; }

        public LevelLoader(ISceneLoader sceneLoader, LifetimeScope applicationScope)
        {
            _sceneLoader = sceneLoader;
            _applicationScope = applicationScope;
        }

        public async UniTask LoadAsync(string levelSceneName, CancellationToken cancellationToken = default)
        {
            CurrentLevelScene = levelSceneName;

            using (LifetimeScope.EnqueueParent(_applicationScope))
            {
                await _sceneLoader.LoadAsync(levelSceneName, cancellationToken);
            }
        }

        // Перезагружает последний загруженный уровень.
        public UniTask RestartAsync(CancellationToken cancellationToken = default)
        {
            return LoadAsync(CurrentLevelScene, cancellationToken);
        }
    }
}
