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

        private static GameObject ResolveContainerContent(IWindowObj windowObj)
        {
            if (windowObj.ContainerContent.IsNotNull())
                return windowObj.ContainerContent;

            if (windowObj is Component component)
                return component.gameObject;
            else
                return null;
        }
    }
}