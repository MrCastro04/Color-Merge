using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_4
{
    public class Level_4_Scope : BaseLevelScope
    {
        protected override void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Level_4_Controller>();
        }
    }
}
