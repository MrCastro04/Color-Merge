using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Save.Scripts;
using Assets.Game.Modules.Core.SceneManagement.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts
{
    // Единственный скоуп игры. Лежит в сцене Game и пересоздаётся при её перезагрузке (рестарт / следующий уровень),
    // поэтому то, что должно пережить перезагрузку (текущий уровень, рекорды), хранится через ISaveService.
    // View'шки берутся из сцены (RegisterComponentInHierarchy) — все они обязаны быть на сцене.
    public class ApplicationScope : LifetimeScope
    {
        [SerializeField] private ItemAssets _itemAssets;
        [SerializeField] private SlotAssets _slotAssets;
        [SerializeField] private LevelAssets _levelAssets;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterServices(builder);

            RegisterAssets(builder);

            RegisterBoard(builder);

            RegisterGenerator(builder);

            RegisterLevel(builder);

            RegisterUI(builder);

            builder.RegisterEntryPoint<LevelController>();
        }

        private static void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<AddressablesAssetProvider>(Lifetime.Singleton).As<IAssetProvider>().AsSelf();
            builder.Register<PlayerPrefsSaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();
            builder.Register<LevelProgress>(Lifetime.Singleton);
        }

        private void RegisterAssets(IContainerBuilder builder)
        {
            builder.RegisterInstance(_itemAssets);
            builder.RegisterInstance(_slotAssets);
            builder.RegisterInstance(_levelAssets);
        }

        private static void RegisterBoard(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<DragLayerView>().As<IDragContext>();
            builder.RegisterComponentInHierarchy<BoardView>();

            builder.Register<ItemFactory>(Lifetime.Singleton).As<IItemFactory>();
            builder.Register<SlotFactory>(Lifetime.Singleton).As<ISlotFactory>();
            builder.Register<BoardController>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        private static void RegisterGenerator(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<GeneratorButtonView>();
            builder.Register<Generator>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
        }

        private static void RegisterLevel(IContainerBuilder builder)
        {
            builder.Register<LevelTimer>(Lifetime.Singleton).AsSelf().AsImplementedInterfaces();
            builder.Register<BestTimeRepository>(Lifetime.Singleton);
        }

        private static void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<HudView>();
            builder.RegisterComponentInHierarchy<WinScreen>();
        }
    }
}
