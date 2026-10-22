using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.CandyBomb.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Level.Scripts.Conditions;
using Assets.Game.Modules.Content.Slot.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Content.GUI.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    public abstract class BaseLevelScope : LifetimeScope
    {
        [SerializeField] private LevelConfigReference _levelConfig;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelConfig);

            RegisterBoard(builder);

            RegisterGenerator(builder);

            builder.Register<LevelTimer>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<LevelConditionsChecker>(Lifetime.Scoped);
            builder.Register<LevelSounds>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<LevelHaptics>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            RegisterUI(builder);

            builder.Register<InputController>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<LevelGameplay>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            RegisterLevel(builder);
        }

        protected abstract void RegisterLevel(IContainerBuilder builder);

        private static void RegisterBoard(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<DragLayerView>().As<IDragContext>();
            builder.RegisterComponentInHierarchy<BoardView>();

            builder.Register<ItemFactory>(Lifetime.Scoped).As<IItemFactory>();
            builder.Register<SlotFactory>(Lifetime.Scoped).As<ISlotFactory>();
            builder.Register<BoardController>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
        }

        private static void RegisterGenerator(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<GeneratorButtonView>();
            builder.Register<GeneratorController>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            builder.RegisterComponentInHierarchy<UpgradeButtonView>();
            builder.Register<PlayerUpgradesInLevel>(Lifetime.Scoped);
            builder.Register<UpgradeGeneratorSystem>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            builder.RegisterComponentInHierarchy<CandyBombButtonView>();
            builder.RegisterComponentInHierarchy<ExplosionView>();
            builder.RegisterComponentInHierarchy<BombDimView>();
            builder.RegisterComponentInHierarchy<ScreenShakeView>();
            builder.Register<CandyBombSystem>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
        }

        private static void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<HudView>();
            builder.RegisterComponentInHierarchy<WinScreen>();
            builder.RegisterComponentInHierarchy<LoseScreen>();
            builder.RegisterComponentInHierarchy<ChainProgressView>();

            builder.RegisterComponentInHierarchy<SettingsView>();
            builder.RegisterComponentInHierarchy<SettingsButtonView>();
            builder.Register<SettingsController>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
        }
    }
}
