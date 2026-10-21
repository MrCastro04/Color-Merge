using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExMoveAnimator
    {
        public static Tween MoveTo(this IMovator movator, Vector3 endPoint, float durationTO = 0.5f)
        {
            var rt = movator.RT;
            return rt.DOMove(endPoint, durationTO);
        }

        public static Tween MoveYoyo(this IMovator movator,Vector3 endPoint,  float durationTO = 0.5f)
        {
            var rt = movator.RT;
            return rt.DOMove(endPoint, durationTO)
                .SetLoops(-1, LoopType.Yoyo);
        }


        public static void ResetYoyo(this IMovator movator )
        {
            movator.RT.DOKill();
        }
    }

    // IMoveArrow (стрелка с направлением) лежит в Content/Tutorial/Arrows — Core не знает про контент.
    public interface IMovator
    {
        public RectTransform RT { get; }
    }
}
