using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_7
{
    // Тутор Level_7 — стіни (заблоковані клітинки): туди не можна поставити предмет, генератор туди не спавнить.
    // Кроки PlayTutorN як у Tutorial_0_Controller; дошка й генератор на час тутора вимкнені.
    public class Level_7_Controller : IAsyncStartable, IWaiter
    {
        private const int HIGHLIGHT_ORDER = 5;

        private readonly LevelGameplay _levelGameplay;
        private readonly TutorScenario _tutorScenario;
        private readonly BoardController _boardController;
        private readonly BoardView _boardView;
        private readonly GeneratorController _generatorController;
        private readonly GeneratorButtonView _generatorButtonView;
        private readonly UpgradeGeneratorSystem _upgradeGeneratorSystem;

        private readonly List<Func<UniTask>> _preTutorCallbacks = new();
        private readonly List<Func<UniTask>> _postTutorCallbacks = new();

        public Level_7_Controller(
            LevelGameplay levelGameplay, TutorScenario tutorScenario, BoardController boardController, BoardView boardView,
            GeneratorController generatorController, GeneratorButtonView generatorButtonView,
            UpgradeGeneratorSystem upgradeGeneratorSystem)
        {
            _levelGameplay = levelGameplay;
            _tutorScenario = tutorScenario;
            _boardController = boardController;
            _boardView = boardView;
            _generatorController = generatorController;
            _generatorButtonView = generatorButtonView;
            _upgradeGeneratorSystem = upgradeGeneratorSystem;
        }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            return _levelGameplay.RunAsync(PlayTutorialAsync, cancellation);
        }

        private async UniTask PlayTutorialAsync(ItemChainConfig chain, CancellationToken ct)
        {
            var wall = FindWall();

            if (wall == null)
                return;

            _boardController.SetInteractable(false);
            _generatorController.SetInteractable(false);
            _upgradeGeneratorSystem.SetInteractable(false);

            await PlayTutor0(wall, ct);
            await PlayTutor1(ct);

            _boardController.SetInteractable(true);
            _generatorController.SetInteractable(true);
            _upgradeGeneratorSystem.SetInteractable(true);
        }

        private async UniTask PlayTutor0(RectTransform wall, CancellationToken ct = default)
        {
            var boardView = _boardView;

            await this.Wait(0.3f, ct);

            _preTutorCallbacks
                .With(() => boardView.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(0).Arrows[0].PointAt(wall));

            await _tutorScenario.RunStepWithCallbacks(0, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await boardView.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor1(CancellationToken ct = default)
        {
            var generatorButton = _generatorButtonView;

            _preTutorCallbacks
                .With(() => generatorButton.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(1).Arrows[0].PointAt(generatorButton.RT));

            await _tutorScenario.RunStepWithCallbacks(1, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await generatorButton.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
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
