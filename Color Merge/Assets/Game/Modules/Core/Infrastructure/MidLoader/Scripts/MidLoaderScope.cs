using Assets.Game.Modules.Content.UI.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MidLoader.Scripts
{
    // Скоуп сцены Mid Loader (как MidLoaderScope в Combo-Dungeon). ScreenFader, LevelLoader, LoadingTarget — из ApplicationScope.
    public class MidLoaderScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<LoadingBarView>();
            builder.RegisterComponentInHierarchy<LoadingTipsView>();

            builder.RegisterEntryPoint<MidLoaderController>();
        }
    }
}
