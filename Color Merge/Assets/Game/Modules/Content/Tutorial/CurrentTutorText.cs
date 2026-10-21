using TMPro;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Tutorial
{
    public class CurrentTutorText : MonoBehaviour, IInitializable
    {
        [SerializeField] private TMP_Text _text;

        private TutorScenario _scenario;

        [Inject]
        public void Construct(TutorScenario tutorScenario)
        {
            _scenario = tutorScenario;
        }

        public void Initialize()
        {
            _scenario.OnTutorStartRun += HandleOnTutorStartRun;
        }

        private void OnDestroy()
        {
            // зависимости могли не внедриться, если скоуп сцены не собрался
            if (_scenario == null) return;

            _scenario.OnTutorStartRun -= HandleOnTutorStartRun;
        }

        private void HandleOnTutorStartRun(Tutor activeTutor)
        {
            _text.text = activeTutor.name;
        }
    }
}