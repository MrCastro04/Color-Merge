using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Content.Tutorial.Tutor_Scenario.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_0
{
    public class Level_0_Scope : BaseLevelScope
    {
        [SerializeField] private TutorScenario _tutorScenario;
        [SerializeField] private MoveableTutorArrow _tutorFinger;

        protected override void RegisterLevel(IContainerBuilder builder)
        {
            builder.RegisterComponent(_tutorScenario).AsSelf().AsImplementedInterfaces();
            builder.RegisterComponent(_tutorFinger);

            builder.RegisterEntryPoint<Level_0_Controller>();
        }
    }
}
