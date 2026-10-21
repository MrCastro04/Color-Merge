using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    // Глобальный резолвер: находит IPulseObject-компонент на GameObject и запускает/останавливает пульс.
    public static class ExPulseResolver
    {
        public static UniTask StartPulseObj(this GameObject go)
        {
            return go.TryGetComponent(out IPulseObject pulseObj)
                ? pulseObj.StartPulse()
                : UniTask.CompletedTask;
        }

        public static async UniTask StopPulseObj(this GameObject go)
        {
            if (go.TryGetComponent(out IPulseObject pulseObj))
                pulseObj.StopPulse();

            await UniTask.CompletedTask;
        }
    }
}
