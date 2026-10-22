using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExPulseResolver
    {
        public static UniTask StartPulseObj(this GameObject go)
        {
            if (go.TryGetComponent(out IPulseObject pulseObj))
                return pulseObj.StartPulse();
            else
                return UniTask.CompletedTask;
        }

        public static async UniTask StopPulseObj(this GameObject go)
        {
            if (go.TryGetComponent(out IPulseObject pulseObj))
                pulseObj.StopPulse();

            await UniTask.CompletedTask;
        }
    }
}
