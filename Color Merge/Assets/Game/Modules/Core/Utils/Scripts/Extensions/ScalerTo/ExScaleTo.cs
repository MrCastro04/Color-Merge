using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExScaleTo
    {
        public static async UniTask ScaleTo(this IScaleable scaleable, Vector3 endResult, float duration,
            CancellationToken token = default)
        {
            await scaleable.GetScaleTween(endResult, duration).AsyncWaitForCompletion();
        }

        // Только собирает твин и отдаёт его — ничего не запускает и не ждёт.
        // Вызывающий сам решает: заавайтить, докрутить .OnComplete/.SetEase, убить и т.д.
        public static Tween GetScaleTween(this IScaleable scaleable, Vector3 endResult, float duration)
        {
            return scaleable.RT.DOScale(endResult, duration);
        }
    }

    public interface IScaleable
    {
        RectTransform RT { get; }
    }
}