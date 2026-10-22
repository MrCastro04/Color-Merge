using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_3
{
    public class Level_3_Scope : BaseLevelScope
    {
        protected override void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Level_3_Controller>();
        }
    }
}
