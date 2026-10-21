using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExFader
    {
        public static async UniTask FadeIn(this IFadeObj fadeObj, CancellationToken ct = default)
        {
            var duration = fadeObj.FadeInDuration;
            var canvasGroup = fadeObj.CanvasGroup;

            if (duration <= 0)
            {
                duration = 0.5f;
            }

            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 0f;

            var tween = canvasGroup.DOFade(1f, duration);

            await tween.AsyncWaitForCompletion();
        }

        public static async UniTask FadeOut(this IFadeObj fadeObj, CancellationToken ct = default)
        {
            var duration = fadeObj.FadeOutDuration;
            var canvasGroup = fadeObj.CanvasGroup;

            if (duration <= 0)
            {
                duration = 0.5f;
            }

            var tween = canvasGroup.DOFade(0f, duration)
                .OnComplete(() => canvasGroup.gameObject.SetActive(false));

            await tween.AsyncWaitForCompletion();
        }
    }
}
