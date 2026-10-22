using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExTween
    {
        public static UniTask AwaitCompletion(this Tween tween, CancellationToken cancellationToken = default)
        {
            if (tween == null || !tween.active)
                return UniTask.CompletedTask;

            var completionSource = new UniTaskCompletionSource();
            var previousOnKill = tween.onKill;

            tween.onKill = () =>
            {
                previousOnKill?.Invoke();
                completionSource.TrySetResult();
            };

            return completionSource.Task.AttachExternalCancellation(cancellationToken);
        }
    }
}
