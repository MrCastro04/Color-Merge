using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Generator.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Tutorial;
using Assets.Game.Modules.Content.UI.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Levels.Level_1
{
    // Тутор Level_1 — таймер: коли стартує, навіщо поспішати (зірки), що буває ліміт часу.
    // Кроки PlayTutorN як у Tutorial_0_Controller; дошка й генератор на час тутора вимкнені.
    public class Level_1_Controller : IAsyncStartable, IWaiter
    {
        private const int HIGHLIGHT_ORDER = 5;

        private readonly LevelGameplay _levelGameplay;
        private readonly TutorScenario _tutorScenario;
        private readonly BoardController _boardController;
        private readonly GeneratorController _generatorController;
        private readonly HudView _hudView;
        private readonly ChainProgressView _chainProgressView;

        private readonly List<Func<UniTask>> _preTutorCallbacks = new();
        private readonly List<Func<UniTask>> _postTutorCallbacks = new();

        public Level_1_Controller(
            LevelGameplay levelGameplay, TutorScenario tutorScenario, BoardController boardController, GeneratorController generatorController,
            HudView hudView, ChainProgressView chainProgressView)
        {
            _levelGameplay = levelGameplay;
            _tutorScenario = tutorScenario;
            _boardController = boardController;
            _generatorController = generatorController;
            _hudView = hudView;
            _chainProgressView = chainProgressView;
        }

        public UniTask StartAsync(CancellationToken cancellation)
        {
            return _levelGameplay.RunAsync(PlayTutorialAsync, cancellation);
        }

        private async UniTask PlayTutorialAsync(ItemChainConfig chain, CancellationToken ct)
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

            _preTutorCallbacks
                .With(() => chainProgress.ChangeSortGroupInObj(HIGHLIGHT_ORDER))
                .With(() => _tutorScenario.GetTutor(1).Arrows[0].PointAt(chainProgress.GoalIcon));

            await _tutorScenario.RunStepWithCallbacks(1, _preTutorCallbacks, _postTutorCallbacks, true, true, ct);
            await chainProgress.ResetSortGroupInObj();
            await this.Wait(0.3f, ct);
        }

        private async UniTask PlayTutor2(CancellationToken ct = default)
        {
            await _tutorScenario.RunStepWithCallbacks(2, ct: ct);
            await this.Wait(0.3f, ct);
        }
    }
}
