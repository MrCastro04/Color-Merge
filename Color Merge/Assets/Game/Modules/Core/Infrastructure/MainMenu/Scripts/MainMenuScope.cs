using Assets.Game.Modules.Content.UI.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MainMenu.Scripts
{
    // Скоуп сцены Main Menu. Общие сервисы (звук, загрузка уровней) приходят из ApplicationScope.
    public class MainMenuScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<MainMenuView>();
            builder.RegisterComponentInHierarchy<SettingsView>();
            builder.RegisterComponentInHierarchy<LevelSelectView>();

            builder.RegisterEntryPoint<MainMenuController>();
        }
    }
}
