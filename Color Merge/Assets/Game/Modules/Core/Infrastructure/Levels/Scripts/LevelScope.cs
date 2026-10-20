using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Content.UI.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Скоуп сцены уровня (как Tutorial_N_Scope в Combo-Dungeon). Общие сервисы приходят из ApplicationScope.
    // Сцена, запущенная напрямую (не через Boot), их не найдёт.
    // View'шки берутся из сцены (RegisterComponentInHierarchy) — все они обязаны быть на сцене.
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private LevelConfigReference _levelConfig;
        [SerializeField] private ItemAssets _itemAssets;
        [SerializeField] private SlotAssets _slotAssets;

        protected override void Configure(IContainerBuilder builder)
        {
            RegisterAssets(builder);

            RegisterBoard(builder);

            RegisterGenerator(builder);

            builder.Register<LevelTimer>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            RegisterUI(builder);

            builder.RegisterEntryPoint<LevelController>();
        }

        private void RegisterAssets(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelConfig);
            builder.RegisterInstance(_itemAssets);
            builder.RegisterInstance(_slotAssets);
        }

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
            builder.Register<Generator>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
        }

        private static void RegisterUI(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<HudView>();
            builder.RegisterComponentInHierarchy<WinScreen>();
        }
    }
}
