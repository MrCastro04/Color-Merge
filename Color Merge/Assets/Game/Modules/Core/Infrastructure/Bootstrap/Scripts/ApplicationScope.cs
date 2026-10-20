using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Save.Scripts;
using Assets.Game.Modules.Core.SceneManagement.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts
{
    // Лежит в Boot.unity и живёт всю игру: общие сервисы (Singleton).
    // Сцены уровней получают их через LevelLoader (LifetimeScope.EnqueueParent), как в Combo-Dungeon.
    public class ApplicationScope : LifetimeScope
    {
        [SerializeField] private LevelCatalog _levelCatalog;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelCatalog);

            builder.Register<AddressablesAssetProvider>(Lifetime.Singleton).As<IAssetProvider>().AsSelf();
            builder.Register<PlayerPrefsSaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();

            RegisterLevels(builder);

            builder.RegisterEntryPoint<BootController>();
        }

        private void RegisterLevels(IContainerBuilder builder)
        {
            builder.Register<LevelProgress>(Lifetime.Singleton);
            builder.Register<BestTimeRepository>(Lifetime.Singleton);
            builder.Register<LevelLoader>(Lifetime.Singleton).WithParameter<LifetimeScope>(this);
        }
    }
}
