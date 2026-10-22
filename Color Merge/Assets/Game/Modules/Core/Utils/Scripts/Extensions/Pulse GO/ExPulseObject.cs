using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExPulseObject
    {
        public static async UniTask StartPulseWithTimer(this IPulseObject pulseObject,
            float seconds, CancellationToken ct = default)
        {
            await UniTask.WaitForSeconds(seconds, cancellationToken: ct);

            await pulseObject.StartPulse();
        }
        
        public static async UniTask StartPulse(this IPulseObject pulseObj,
            bool? withRestart = false, float? restartTime = null)
        {
            if (pulseObj is IPulseObjWithPointer pointerObj && pointerObj.HasPointer)
            {
                pointerObj.RT?.DOKill();
                pointerObj.PulseSequence?.Kill(true);

                if (withRestart == true && restartTime.HasValue)
                {
                    pulseObj.StartPulseWithTimer(restartTime.Value).Forget();
                }

                return;
            }

            const Ease defaultPulseEase = Ease.InOutSine;
            const float defaultMinPulseScale = 1f;
            const float defaultMaxPulseScale = 1.25f;
            const float defaultPulseDuration = 0.5f;

            var minScale = defaultMinPulseScale;
            var maxScale = defaultMaxPulseScale;
            var duration = defaultPulseDuration;
            var ease = defaultPulseEase;

            if (pulseObj.MinPulseScale != 0f)
                minScale = pulseObj.MinPulseScale;

            if (pulseObj.MaxPulseScale != 0f)
                maxScale = pulseObj.MaxPulseScale;

            if (pulseObj.PulseDuration != 0f)
                duration = pulseObj.PulseDuration;

            if (pulseObj.PulseEase != default)
                ease = pulseObj.PulseEase;
            var rt = ResolveRT(pulseObj);

            rt.localScale = Vector3.one * minScale;
            pulseObj.PulseSequence = DOTween.Sequence();

            var scaleBig = GetTweenScale(rt, maxScale, duration, ease);
            var scaleSmall = GetTweenScale(rt, minScale, duration, ease);

            pulseObj.PulseSequence
                .Append(scaleBig)
                .Append(scaleSmall)
                .SetLoops(-1, LoopType.Yoyo)
                .Play();

            await UniTask.CompletedTask;
        }

        public static void StopPulse(this IPulseObject pulseObj)
        {
            var isCard = pulseObj is IPulseObjWithPointer;
            pulseObj.PulseSequence?.Kill(isCard);
        }

        private static Tween GetTweenScale(RectTransform rt, float scaleTo, float duration, Ease ease)
        {
            return rt
                .DOScale(scaleTo, duration)
                .SetEase(ease);
        }

        private static RectTransform ResolveRT(IPulseObject pulseObj)
        {
            if (pulseObj.RT.IsNotNull())
                return pulseObj.RT;

            if (pulseObj is Component component)
                return component.GetComponent<RectTransform>();
            else
                return null;
        }
    }
}