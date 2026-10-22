using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Content.Tutorial.Tutor_Scenario.Scripts;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts
{
    // Тутор Level_5 — цукерка (Candy_Connector): що робить, навчальний мердж у цукерку (палець), мета — N мерджів.
    // Кроки PlayTutorN як у Tutorial_0_Controller; дошка й генератор на час тутора вимкнені.
    public class Tutorial_3_Controller : ILevelTutorial, IWaiter
    {
        private const int HIGHLIGHT_ORDER = 5;

        private readonly TutorScenario _tutorScenario;
        private readonly MoveableTutorArrow _finger;
        private readonly BoardController _boardController;
        private readonly BoardView _boardView;
        private readonly GeneratorController _generatorController;
        private readonly HudView _hudView;

        private readonly List<Func<UniTask>> _preTutorCallbacks = new();
        private readonly List<Func<UniTask>> _postTutorCallbacks = new();

        private ItemChainConfig _chain;
        private ItemController _candy;
        private ItemController _item;

        public Tutorial_3_Controller(
            TutorScenario tutorScenario, MoveableTutorArrow finger,
            BoardController boardController, BoardView boardView,
            GeneratorController generatorController, HudView hudView)
        {
            _tutorScenario = tutorScenario;
            _finger = finger;
            _boardController = boardController;
            _boardView = boardView;
            _generatorController = generatorController;
            _hudView = hudView;
        }

        public async UniTask PlayAsync(ItemChainConfig chain, CancellationToken ct = default)
        {
            _chain = chain;
            _finger.gameObject.SetActive(false);

            _boardController.SetInteractable(false);
            _generatorController.SetInteractable(false);

            if (!TryFindCandyAndItem(out _candy, out _item))
                return;

            await PlayTutor0(ct);
            await PlayTutor1(ct);
            await PlayTutor2(ct);

            _boardController.SetInteractable(true);
            _generatorController.SetInteractable(true);
        }

        private async UniTask PlayTutor0(CancellationToken ct = default)
        {
            var boardView = _boardView;

            await this.Wait(0.3f, ct);

            _preTutorCallbacks
                .With(() => boardView.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(0).Arrows[0].PointAt(_candy.View.RT));

            await _tutorScenario.RunStepWithCallbacks(0, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await boardView.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor1(CancellationToken ct = default)
        {
            var boardView = _boardView;

            _preTutorCallbacks
                .With(() => boardView.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(AllowOnlyCandyPair);

            _postTutorCallbacks.With(ShowFinger);

            await _tutorScenario.RunStepWithCallbacks(1, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);

            HideFinger();
            _boardController.SetInteractable(false);
            await boardView.ResetSortGroupInObj();
            await this.Wait(0.5f, ct);
        }

        private async UniTask PlayTutor2(CancellationToken ct = default)
        {
            var hud = _hudView;

            _preTutorCallbacks
                .With(() => hud.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(2).Arrows[0].PointAt(hud.MergesTextRT));

            await _tutorScenario.RunStepWithCallbacks(2, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await hud.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private bool TryFindCandyAndItem(out ItemController candy, out ItemController item)
        {
            candy = null;
            item = null;
            int bestLevel = int.MaxValue;

            foreach (var boardItem in _boardController.GetItems())
            {
                if (BoardController.IsCandy(boardItem))
                {
                    candy ??= boardItem;
                    continue;
                }

                int level = _chain.GetLevel(boardItem.Config);

                if (level > 0 && level < bestLevel && _chain.TryGetNext(boardItem.Config, out _))
                {
                    bestLevel = level;
                    item = boardItem;
                }
            }

            return candy != null && item != null;
        }

        private void AllowOnlyCandyPair()
        {
            _boardController.SetInteractable(true);

            foreach (var boardItem in _boardController.GetItems())
            {
                if (boardItem != _candy && boardItem != _item)
                    boardItem.View.SetInteractable(false);
            }
        }

        private void ShowFinger()
        {
            _finger.gameObject.SetActive(true);
            _finger.Play(_item.View.RT.position, _candy.View.RT.position);
        }

        private void HideFinger()
        {
            _finger.Stop();
            _finger.gameObject.SetActive(false);
        }
    }
}
