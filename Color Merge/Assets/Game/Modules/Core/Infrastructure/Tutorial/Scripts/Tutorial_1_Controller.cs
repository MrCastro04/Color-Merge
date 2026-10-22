using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts
{
    // Тутор Level_1 — таймер: когда стартует, зачем спешить (звёзды), что бывает лимит времени.
    // Шаги PlayTutorN как в Tutorial_0_Controller; доска и генератор на время тутора выключены.
    public class Tutorial_1_Controller : ILevelTutorial, IWaiter
    {
        // Затемнение — слой Tutor, order 1, текст — order 10; подсвеченный объект между ними.
        private const int HIGHLIGHT_ORDER = 5;

        private readonly TutorScenario _tutorScenario;
        private readonly BoardController _boardController;
        private readonly GeneratorController _generatorController;
        private readonly HudView _hudView;
        private readonly ChainProgressView _chainProgressView;

        private readonly List<Func<UniTask>> _preTutorCallbacks = new();
        private readonly List<Func<UniTask>> _postTutorCallbacks = new();

        public Tutorial_1_Controller(
            TutorScenario tutorScenario, BoardController boardController, GeneratorController generatorController,
            HudView hudView, ChainProgressView chainProgressView)
        {
            _tutorScenario = tutorScenario;
            _boardController = boardController;
            _generatorController = generatorController;
            _hudView = hudView;
            _chainProgressView = chainProgressView;
        }

        public async UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default)
        {
            _boardController.SetInteractable(false);
            _generatorController.SetInteractable(false);

            await PlayTutor0(ct);
            await PlayTutor1(ct);
            await PlayTutor2(ct);

            _boardController.SetInteractable(true);
            _generatorController.SetInteractable(true);
        }

        private async UniTask PlayTutor0(CancellationToken ct = default)
        {
            var hud = _hudView;

            await this.Wait(0.3f, ct);

            // тутор - это таймер, он стартует с первого мерджа
            _preTutorCallbacks
                .With(() => hud.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(0).Arrows[0].PointAt(hud.TimeTextWorldBounds));

            await _tutorScenario.RunStepWithCallbacks(0, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await hud.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor1(CancellationToken ct = default)
        {
            var chainProgress = _chainProgressView;

            // тутор - собери цель быстрее: больше звёзд
            _preTutorCallbacks
                .With(() => chainProgress.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(1).Arrows[0].PointAt(chainProgress.GoalIcon));

            await _tutorScenario.RunStepWithCallbacks(1, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await chainProgress.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor2(CancellationToken ct = default)
        {
            // тутор - на некоторых уровнях лимит времени, таймер идёт назад
            await _tutorScenario.RunStepWithCallbacks(2, ct: ct);
            await this.Wait(0.3f, ct);
        }
    }
}
