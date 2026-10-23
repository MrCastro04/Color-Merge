using Assets.Game.Modules.Content.GUI.Loading.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.MidLoader.Scripts
{
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
