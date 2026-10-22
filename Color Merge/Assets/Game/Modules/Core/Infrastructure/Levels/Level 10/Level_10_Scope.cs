using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_10
{
    public class Level_10_Scope : BaseLevelScope
    {
        protected override void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Level_10_Controller>();
        }
    }
}
