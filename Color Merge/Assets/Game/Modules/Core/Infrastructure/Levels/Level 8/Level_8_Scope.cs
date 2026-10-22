using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_8
{
    public class Level_8_Scope : BaseLevelScope
    {
        protected override void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Level_8_Controller>();
        }
    }
}
