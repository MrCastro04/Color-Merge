using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.CandyBomb.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts
{
    // Тутор Level_9 — цукеркова бомба: кнопка й енергія, що робить вибух, безкоштовний заряд і перший вибух
    // (крок закривається вибухом — SkipCondition.External). Кроки PlayTutorN як у Tutorial_0_Controller.
    public class Tutorial_5_Controller : ILevelTutorial, IWaiter
    {
        private const int HIGHLIGHT_ORDER = 5;

        private readonly TutorScenario _tutorScenario;
        private readonly BoardController _boardController;
        private readonly BoardView _boardView;
        private readonly GeneratorController _generatorController;
        private readonly CandyBombSystem _candyBombSystem;
        private readonly CandyBombButtonView _bombButtonView;

        private readonly List<Func<UniTask>> _preTutorCallbacks = new();
        private readonly List<Func<UniTask>> _postTutorCallbacks = new();

        public Tutorial_5_Controller(
            TutorScenario tutorScenario, BoardController boardController, BoardView boardView,
            GeneratorController generatorController, CandyBombSystem candyBombSystem, CandyBombButtonView bombButtonView)
        {
            _tutorScenario = tutorScenario;
            _boardController = boardController;
            _boardView = boardView;
            _generatorController = generatorController;
            _candyBombSystem = candyBombSystem;
            _bombButtonView = bombButtonView;
        }

        public async UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default)
        {
            if (!_candyBombSystem.IsEnabled)
                return;

            _boardController.SetInteractable(false);
            _generatorController.SetInteractable(false);

            await PlayTutor0(ct);
            await PlayTutor1(ct);
            await PlayTutor2(ct);

            _boardController.SetInteractable(true);
            _generatorController.SetInteractable(true);
            _candyBombSystem.SetInteractable(true);
        }

        private async UniTask PlayTutor0(CancellationToken ct = default)
        {
            var bombButton = _bombButtonView;

            await this.Wait(0.3f, ct);

            _preTutorCallbacks
                .With(() => bombButton.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(0).Arrows[0].PointAt(bombButton.RT));

            await _tutorScenario.RunStepWithCallbacks(0, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await bombButton.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor1(CancellationToken ct = default)
        {
            var boardView = _boardView;
            var wall = FindWall();

            _preTutorCallbacks.With(() => boardView.ChangeSortGroupInObj(HIGHLIGHT_ORDER));

            if (wall != null)
                _preTutorCallbacks.With(() => _tutorScenario.GetTutor(1).Arrows[0].PointAt(wall));

            await _tutorScenario.RunStepWithCallbacks(1, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await boardView.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor2(CancellationToken ct = default)
        {
            var boardView = _boardView;
            var bombButton = _bombButtonView;

            _preTutorCallbacks
                .With(() => _candyBombSystem.AddEnergy(_candyBombSystem.Cost))
                .With(() => boardView.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => bombButton.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(AllowOnlyBomb)
                .With(() => _tutorScenario.GetTutor(2).Arrows[0].PointAt(bombButton.RT));

            _candyBombSystem.OnExploded += _tutorScenario.RequestSkip;

            await _tutorScenario.RunStepWithCallbacks(2, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);

            _candyBombSystem.OnExploded -= _tutorScenario.RequestSkip;

            await boardView.ResetSortGroupInObj();
            await bombButton.ResetSortGroupInObj();
            await this.Wait(0.5f, ct);
        }

        private void AllowOnlyBomb()
        {
            _boardController.SetInteractable(true);

            foreach (var item in _boardController.GetItems())
                item.View.SetInteractable(false);

            _candyBombSystem.SetInteractable(true);
        }

        private RectTransform FindWall()
        {
            foreach (var slot in _boardController.Slots)
            {
                if (slot.IsBlocked)
                    return (RectTransform)slot.View.transform;
            }

            return null;
        }
    }
}
