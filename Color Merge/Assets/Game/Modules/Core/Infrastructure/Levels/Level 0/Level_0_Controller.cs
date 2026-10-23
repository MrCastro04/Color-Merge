using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.GUI.HUD.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Content.Tutorial.Tutor_Scenario.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_0
{
    // Тутор навчального рівня Level_0 (як Tutorial_0_Controller у Combo-Dungeon): кроки PlayTutorN по черзі,
    // до/після тексту — колбеки (підсвічування через сортинг-шар Tutor, пульс, палець, стрілки).
    // Level_0 і є навчанням — тутор грає під час кожного входу на рівень.
    public class Level_0_Controller : IAsyncStartable, IWaiter
    {
        private const int HIGHLIGHT_ORDER = 5;
        private const int FINGER_REFRESH_DELAY_MS = 350;

        private readonly LevelGameplay _levelGameplay;
        private readonly TutorScenario _tutorScenario;
        private readonly MoveableTutorArrow _finger;
        private readonly BoardController _boardController;
        private readonly BoardView _boardView;
        private readonly GeneratorController _generatorController;
        private readonly GeneratorButtonView _generatorButtonView;
        private readonly ChainProgressView _chainProgressView;

        private readonly List<Func<UniTask>> _preTutorCallbacks = new();
        private readonly List<Func<UniTask>> _postTutorCallbacks = new();

        private ItemChainConfig _chain;
        private ItemController _mergeFrom;
        private ItemController _mergeTo;

        public Level_0_Controller(
            LevelGameplay levelGameplay, TutorScenario tutorScenario, MoveableTutorArrow finger,
            BoardController boardController, BoardView boardView,
            GeneratorController generatorController, GeneratorButtonView generatorButtonView,
            ChainProgressView chainProgressView)
        {
            _levelGameplay = levelGameplay;
            _tutorScenario = tutorScenario;
            _finger = finger;
            _boardController = boardController;
            _boardView = boardView;
            _generatorController = generatorController;
            _generatorButtonView = generatorButtonView;
            _chainProgressView = chainProgressView;
        }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            return _levelGameplay.RunAsync(PlayTutorialAsync, cancellation);
        }

        private async UniTask PlayTutorialAsync(ItemChainConfig chain, CancellationToken ct)
        {
            _chain = chain;
            _finger.gameObject.SetActive(false);

            _boardController.SetInteractable(false);
            _generatorController.SetInteractable(false);

            await PlayTutor0(ct);
            await PlayTutor1(ct);
            await PlayTutor2(ct);
            await PlayTutor3(ct);
            await PlayTutor4(ct);

            _boardController.SetInteractable(true);
            _generatorController.SetInteractable(true);
        }

        private async UniTask PlayTutor0(CancellationToken ct = default)
        {
            await this.Wait(0.3f, ct);

            await _tutorScenario.RunStepWithCallbacks(0, ct: ct);
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor1(CancellationToken ct = default)
        {
            var chainProgress = _chainProgressView;

            _preTutorCallbacks
                .With(() => chainProgress.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => PointArrows(1, chainProgress.GoalIcon, chainProgress.FirstIcon));
            await _tutorScenario.RunStepWithCallbacks(1, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await chainProgress.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor2(CancellationToken ct = default)
        {
            if (!TryFindMergePair(out _mergeFrom, out _mergeTo))
                return;

            var boardView = _boardView;

            _preTutorCallbacks
                .With(() => boardView.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(AllowOnlyMergePair);

            _postTutorCallbacks.With(ShowFinger);

            _boardController.OnItemMoved += HandleOnPairItemMoved;
            _boardController.OnItemsSwapped += HandleOnPairItemsSwapped;

            await _tutorScenario.RunStepWithCallbacks(2, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);

            _boardController.OnItemMoved -= HandleOnPairItemMoved;
            _boardController.OnItemsSwapped -= HandleOnPairItemsSwapped;

            HideFinger();
            _boardController.SetInteractable(false);
            await boardView.ResetSortGroupInObj();
            await this.Wait(0.5f, ct);
        }

        private async UniTask PlayTutor3(CancellationToken ct = default)
        {
            var generatorButton = _generatorButtonView;

            _preTutorCallbacks
                .With(() => generatorButton.gameObject.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => PointArrows(3, generatorButton.RT, generatorButton.RT))
                .With(() => _generatorController.SetInteractable(true));

            _postTutorCallbacks.With(() => generatorButton.gameObject.StartPulseObj());

            await _tutorScenario.RunStepWithCallbacks(3, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await generatorButton.gameObject.StopPulseObj();
            generatorButton.transform.localScale = Vector3.one;

            _generatorController.SetInteractable(false);
            await generatorButton.gameObject.ResetSortGroupInObj();
            await this.Wait(0.5f, ct);
        }

        private async UniTask PlayTutor4(CancellationToken ct = default)
        {
            var chainProgress = _chainProgressView;

            _preTutorCallbacks
                .With(() => chainProgress.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => PointArrows(4, chainProgress.GoalIcon));

            await _tutorScenario.RunStepWithCallbacks(4, _preTutorCallbacks, _postTutorCallbacks, ct: ct);
            await chainProgress.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private bool TryFindMergePair(out ItemController from, out ItemController to)
        {
            var items = _boardController.GetItems();
            int bestLevel = int.MaxValue;

            from = null;
            to = null;

            for (int i = 0; i < items.Count; i++)
            {
                for (int j = i + 1; j < items.Count; j++)
                {
                    if (!items[i].IsSameAs(items[j]))
                        continue;

                    int level = _chain.GetLevel(items[i].Config);

                    if (level <= 0 || level >= bestLevel)
                        continue;

                    bestLevel = level;
                    from = items[i];
                    to = items[j];
                }
            }

            return from != null;
        }

        private void AllowOnlyMergePair()
        {
            _boardController.SetInteractable(true);

            foreach (var item in _boardController.GetItems())
            {
                if (item != _mergeFrom && item != _mergeTo)
                    item.View.SetInteractable(false);
            }
        }

        private void PointArrows(int step, params RectTransform[] targets)
        {
            var arrows = _tutorScenario.GetTutor(step).Arrows;

            for (int i = 0; i < arrows.Count && i < targets.Length; i++)
            {
                if (targets[i] != null)
                    arrows[i].PointAt(targets[i]);
            }
        }

        private void ShowFinger()
        {
            _finger.gameObject.SetActive(true);
            _finger.Play(_mergeFrom.View.RT.position, _mergeTo.View.RT.position);
        }

        private void HideFinger()
        {
            _finger.Stop();
            _finger.gameObject.SetActive(false);
        }

        private void HandleOnPairItemMoved(ItemController item)
        {
            if (item == _mergeFrom || item == _mergeTo)
                ShowFingerAfterMove().Forget();
        }

        private void HandleOnPairItemsSwapped(ItemController first, ItemController second)
        {
            HandleOnPairItemMoved(first);
            HandleOnPairItemMoved(second);
        }

        private async UniTaskVoid ShowFingerAfterMove()
        {
            await UniTask.Delay(FINGER_REFRESH_DELAY_MS);

            if (_finger != null && _finger.gameObject.activeSelf)
                ShowFinger();
        }
    }
}