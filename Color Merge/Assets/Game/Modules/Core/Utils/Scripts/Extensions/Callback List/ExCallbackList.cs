using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExCallbackList
    {
        // Возвращает тот же список — благодаря этому вызовы можно цеплять цепочкой:
        // PRECALL.With(() => ...).With(() => ...).With(() => ...);
        public static List<Func<UniTask>> With(this List<Func<UniTask>> callbacks, Func<UniTask> action)
        {
            callbacks.Add(action);
            return callbacks;
        }

        // Перегрузка для синхронных действий (void), чтобы не оборачивать их в () => { x(); return UniTask.CompletedTask; } вручную.
        public static List<Func<UniTask>> With(this List<Func<UniTask>> callbacks, Action action)
        {
            callbacks.Add(() =>
            {
                action();
                return UniTask.CompletedTask;
            });

            return callbacks;
        }
    }
}
