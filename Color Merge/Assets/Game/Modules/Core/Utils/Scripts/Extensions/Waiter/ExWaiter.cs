using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExWaiter
    {
        public static async UniTask Wait(this IWaiter waiter, float seconds, CancellationToken cancellationToken = default)
        {
            await UniTask.WaitForSeconds(seconds, cancellationToken: cancellationToken);
        }

        public static async UniTask WaitUntil(this IWaiter waiter, Func<bool> condition, CancellationToken cancellationToken = default)
        {
            await UniTask.WaitUntil(condition, cancellationToken: cancellationToken);
        }

        // Склеивает несколько ожиданий в одно: await a.Wait(1f).With(b.Wait(2f), c.WaitUntil(() => flag));
        public static UniTask With(this UniTask task, params UniTask[] otherTasks)
        {
            if (otherTasks.IsNull() || otherTasks.Length == 0)
                return task;

            var allTasks = new UniTask[otherTasks.Length + 1];
            allTasks[0] = task;

            Array.Copy(otherTasks, 0, allTasks, 1, otherTasks.Length);

            return UniTask.WhenAll(allTasks);
        }
    }
}
