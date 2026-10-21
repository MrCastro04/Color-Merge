using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts.Interfaces;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Content.Tutorial.Tutor_Scenario.Scripts;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Scripts
{
    // Скоуп сцены уровня (как Tutorial_N_Scope в Combo-Dungeon). В инспекторе — только конфиг уровня,
    // общие сервисы и ассеты приходят из ApplicationScope. Сцена, запущенная напрямую (не через Boot), их не найдёт.
    // View'шки берутся из сцены (RegisterComponentInHierarchy) — все они обязаны быть на сцене.
    public class LevelScope : LifetimeScope
    {
        [SerializeField] private LevelConfigReference _levelConfig;

        [Header("Тутор — только на уровне, где он есть (Level_1), иначе пусто")]
        [SerializeField] private TutorScenario _tutorScenario;
        [SerializeField] private MoveableTutorArrow _tutorFinger;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_levelConfig);

            RegisterBoard(builder);

            RegisterGenerator(builder);

            builder.Register<LevelTimer>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
            builder.Register<LevelSounds>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            RegisterUI(builder);

            builder.Register<InputController>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();

            RegisterTutor(builder);

            builder.RegisterEntryPoint<LevelController>();
        }

        private void RegisterTutor(IContainerBuilder builder)
        {
            if (_tutorScenario == null)
            {
                builder.Register<EmptyLevelTutorial>(Lifetime.Scoped).As<ILevelTutorial>();
                return;
            }

            builder.RegisterComponent(_tutorScenario).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_tutorFinger);
            builder.Register<Tutorial_1_Controller>(Lifetime.Scoped).As<ILevelTutorial>();
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

            builder.RegisterComponentInHierarchy<UpgradeButtonView>();
            builder.Register<PlayerUpgradesInLevel>(Lifetime.Scoped);
            builder.Register<UpgradeGeneratorSystem>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
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
