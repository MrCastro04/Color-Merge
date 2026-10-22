using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExWindowOBJ
    {
        public static async UniTask OpenAsWindow(this IWindowObj windowObj, CancellationToken ct = default, Action callback = null)
        {
            var openDuration = windowObj.OpenDuration;
            var containerContent = ResolveContainerContent(windowObj);

            if (!containerContent.activeSelf)
                containerContent.SetActive(true);

            if (openDuration <= 0)
            {
                openDuration = 0.5f;
            }

            var tween = containerContent.gameObject.transform
                .DOScale(Vector3.one, openDuration)
                .From(Vector3.zero)
                .OnStart(() =>
                {
                    containerContent.SetActive(true);
                    windowObj.IsOpened = containerContent.gameObject.activeSelf;
                });

            await tween.AsyncWaitForCompletion();
            
            callback?.Invoke();
        }

        public static async UniTask CloseAsWindow(this IWindowObj windowObj, CancellationToken ct = default)
        {
            var closeDuration = windowObj.CloseDuration;
            var containerContent = ResolveContainerContent(windowObj);

            if (closeDuration <= 0)
            {
                closeDuration = 0.5f;
            }

            var tween = containerContent.gameObject.transform
                .DOScale(Vector3.zero, closeDuration)
                .From(Vector3.one)
                .OnComplete(() =>
                {
                    containerContent.SetActive(false);
                    windowObj.IsOpened = containerContent.gameObject.activeSelf;
                });

            await tween.AsyncWaitForCompletion();
        }

        private static GameObject ResolveContainerContent(IWindowObj windowObj)
        {
            if (windowObj.ContainerContent.IsNotNull())
                return windowObj.ContainerContent;

            return windowObj is Component component ? component.gameObject : null;
        }
    }
}