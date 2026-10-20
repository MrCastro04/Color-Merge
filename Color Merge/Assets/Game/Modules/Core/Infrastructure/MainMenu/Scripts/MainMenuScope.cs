using Assets.Game.Modules.Content.UI.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MainMenu.Scripts
{
    // Скоуп сцены Main Menu. Общие сервисы (звук, загрузка уровней, шторка) приходят из ApplicationScope.
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MainMenuView>();
            builder.RegisterComponentInHierarchy<LevelSelectView>();

            RegisterSettings(builder);

            builder.RegisterEntryPoint<MainMenuController>();
        }

        private static void RegisterSettings(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<SettingsView>();
            builder.RegisterComponentInHierarchy<SettingsButtonView>();
            builder.Register<SettingsController>(Lifetime.Scoped).AsSelf().AsImplementedInterfaces();
        }
    }
}
