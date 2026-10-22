using System;
using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Tutorial
{
    public class TutorScenario : MonoBehaviour, IInitializable
    {
        [SerializeField] private TutorStep[] _steps;
        [SerializeField] private Image _background;
        private InputController _inputController;
        private BoardController _boardController;

        public float BackgroundFadeDuration = 0.3f;
        public float DelayAfterBackgroundFade = 0.5f;
        public float BackgroundTargetAlpha = 200f / 255f;

        private CancellationTokenSource _cts;
        private Tutor _activeTutor;
        private TutorStep _activeStep;
        private bool _skipRequested;

        public event Action<Tutor> OnTutorStartRun;

        [Inject]
        private void Construct(InputController inputController, BoardController boardController)
        {
            _inputController = inputController;
            _boardController = boardController;
        }

        public void Initialize()
        {
            _cts = new CancellationTokenSource();

            _inputController.OnTap += HandleTap;

            _background.gameObject.SetActive(false);

            SetVisibleAll(false);
        }

        public void SetVisibleAll(bool value)
        {
            foreach (TutorStep step in _steps)
            {
                if (step.Tutor != null)
                    step.Tutor.SetVisible(value);
            }
        }

        public Tutor GetTutor(int index) => _steps[index].Tutor;

        private void OnDisable()
        {
            if (_inputController != null)
                _inputController.OnTap -= HandleTap;

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;

            if (_activeStep.SkipCondition == SkipCondition.ItemMerged && _boardController != null)
            {
                _boardController.OnItemMerged -= HandleItemMerged;
            }

            if (_activeStep.SkipCondition == SkipCondition.ButtonClick &&
                _activeStep.RequiredButton != null)
            {
                _activeStep.RequiredButton.onClick.RemoveListener(HandleButtonClick);
            }

            if (_activeTutor != null)
            {
                _activeTutor.SetVisible(false);
            }
        }

        public async UniTask RunStepWithCallbacks(int index,
            List<Func<UniTask>> PRECallbacks = null,
            List<Func<UniTask>> POSTCallbacks = null,
            bool? useBackgroundFade = null, bool? showArrowsAfterText = true,
            CancellationToken ct = default)
        {
            CancellationToken token = ResolveToken(ct);

            if (PRECallbacks.IsNotNull() && PRECallbacks.Count > 0)
            {
                foreach (var callback in PRECallbacks)
                {
                    await callback();
                }
            }

            await RunStep(_steps[index], POSTCallbacks, useBackgroundFade, token);

            PRECallbacks?.Clear();
            POSTCallbacks?.Clear();
        }

        private async UniTask RunStep(TutorStep step,
            List<Func<UniTask>> afterShowCallbacks = null,
            bool? useBackgroundFade = null,
            CancellationToken ct = default)
        {
            _activeStep = step;
            _activeTutor = step.Tutor;
            _skipRequested = false;

            OnTutorStartRun?.Invoke(_activeTutor);

            bool shouldFadeBackground = useBackgroundFade ?? step.UseBackgroundFade;

            if (step.SkipCondition == SkipCondition.ItemMerged)
            {
                _boardController.OnItemMerged += HandleItemMerged;
            }

            if (step.SkipCondition == SkipCondition.ButtonClick)
            {
                step.RequiredButton.onClick.AddListener(HandleButtonClick);
            }

            if (shouldFadeBackground)
            {
                await FadeInBackground(ct);
            }

            await _activeTutor.ShowText(ct);

            if (afterShowCallbacks != null && afterShowCallbacks.Count > 0)
            {
                foreach (var callback in afterShowCallbacks)
                {
                    await callback();
                }
            }

            await UniTask.WaitUntil(() => _skipRequested, cancellationToken: ct);

            if (step.SkipCondition == SkipCondition.ItemMerged)
            {
                _boardController.OnItemMerged -= HandleItemMerged;
            }

            if (step.SkipCondition == SkipCondition.ButtonClick)
            {
                step.RequiredButton.onClick.RemoveListener(HandleButtonClick);
            }

            await _activeTutor.Hide();

            if (shouldFadeBackground)
            {
                await FadeOutBackground(ct);
            }

            _activeTutor = null;
        }

        private async UniTask FadeInBackground(CancellationToken ct = default)
        {
            _background.gameObject.SetActive(true);

            Color color = _background.color;
            _background.color = new Color(color.r, color.g, color.b, 0f);

            await _background.DOFade(BackgroundTargetAlpha, BackgroundFadeDuration).SetLink(gameObject).AwaitCompletion(ct);

            await UniTask.Delay(TimeSpan.FromSeconds(DelayAfterBackgroundFade), cancellationToken: ct);
        }

        private async UniTask FadeOutBackground(CancellationToken ct = default)
        {
            await _background.DOFade(0f, BackgroundFadeDuration).SetLink(gameObject).AwaitCompletion(ct);

            ct.ThrowIfCancellationRequested();

            _background.gameObject.SetActive(false);
        }

        private CancellationToken ResolveToken(CancellationToken ct)
        {
            if (ct != default)
            {
                return ct;
            }

            return _cts?.Token ?? default;
        }

        private void HandleTap()
        {
            if (_activeTutor == null) return;

            if (_activeTutor.IsTyping)
            {
                _activeTutor.SkipTyping();
            }
            else if (_activeStep.SkipCondition == SkipCondition.Tap)
            {
                _skipRequested = true;
            }
        }

        private void HandleItemMerged(ItemController item)
        {
            _skipRequested = true;
        }

        private void HandleButtonClick()
        {
            _skipRequested = true;
        }

        public void RequestSkip()
        {
            if (_activeTutor != null && _activeStep.SkipCondition == SkipCondition.External)
                _skipRequested = true;
        }
    }
}
