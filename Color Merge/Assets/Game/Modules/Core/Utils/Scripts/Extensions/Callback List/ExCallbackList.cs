using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExCallbackList
    {
        public static List<Func<UniTask>> With(this List<Func<UniTask>> callbacks, Func<UniTask> action)
        {
            callbacks.Add(action);
            return callbacks;
        }

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
