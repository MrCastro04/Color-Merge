using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts
{
    // Тутор Level_2 — кнопка улучшения генератора: как копится энергия, что даёт улучшение, цель уровня.
    // Шаги PlayTutorN как в Tutorial_0_Controller; доска, генератор и кнопка на время тутора выключены.
    public class Tutorial_2_Controller : ILevelTutorial, IWaiter
    {
        private const int HIGHLIGHT_ORDER = 5;

        private readonly TutorScenario _tutorScenario;
        private readonly BoardController _boardController;
        private readonly GeneratorController _generatorController;
        private readonly GeneratorButtonView _generatorButtonView;
        private readonly UpgradeGeneratorSystem _upgradeGeneratorSystem;
        private readonly UpgradeButtonView _upgradeButtonView;

        private readonly List<Func<UniTask>> _preTutorCallbacks = new();
        private readonly List<Func<UniTask>> _postTutorCallbacks = new();

        public Tutorial_2_Controller(
            TutorScenario tutorScenario, BoardController boardController,
            GeneratorController generatorController, GeneratorButtonView generatorButtonView,
            UpgradeGeneratorSystem upgradeGeneratorSystem, UpgradeButtonView upgradeButtonView)
        {
            _tutorScenario = tutorScenario;
            _boardController = boardController;
            _generatorController = generatorController;
            _generatorButtonView = generatorButtonView;
            _upgradeGeneratorSystem = upgradeGeneratorSystem;
            _upgradeButtonView = upgradeButtonView;
        }

        public async UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default)
        {
            _boardController.SetInteractable(false);
            _generatorController.SetInteractable(false);
            _upgradeGeneratorSystem.SetInteractable(false);

            await PlayTutor0(ct);
            await PlayTutor1(ct);
            await PlayTutor2(ct);

            _boardController.SetInteractable(true);
            _generatorController.SetInteractable(true);
            _upgradeGeneratorSystem.SetInteractable(true);
        }

        private async UniTask PlayTutor0(CancellationToken ct = default)
        {
            var upgradeButton = _upgradeButtonView;
            var generatorButton = _generatorButtonView;

            await this.Wait(0.3f, ct);

            _preTutorCallbacks
                .With(() => upgradeButton.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => generatorButton.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => PointArrows(0, generatorButton.RT, upgradeButton.LabelRT));

            await _tutorScenario.RunStepWithCallbacks(0, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await upgradeButton.ResetSortGroupInObj();
            await generatorButton.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor1(CancellationToken ct = default)
        {
            var upgradeButton = _upgradeButtonView;

            _preTutorCallbacks
                .With(() => upgradeButton.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => PointArrows(1, upgradeButton.LabelRT));

            await _tutorScenario.RunStepWithCallbacks(1, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await upgradeButton.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor2(CancellationToken ct = default)
        {
            var generatorButton = _generatorButtonView;

            _preTutorCallbacks
                .With(() => generatorButton.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => PointArrows(2, generatorButton.RT));

            await _tutorScenario.RunStepWithCallbacks(2, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await generatorButton.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private void PointArrows(int step, params UnityEngine.RectTransform[] targets)
        {
            var arrows = _tutorScenario.GetTutor(step).Arrows;

            for (int i = 0; i < arrows.Count && i < targets.Length; i++)
            {
                if (targets[i] != null)
                    arrows[i].PointAt(targets[i]);
            }
        }
    }
}
