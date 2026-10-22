using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public static class ExChangeSortGroupResolver
    {
        public static async UniTask ChangeSortGroupInObj(this object obj, int orderInLayer = 0)
        {
            if (TryResolve(obj, out IChangeSortGroupInCanvas canvasObj))
            {
                if (canvasObj.DesiredGroup == String.Empty)
                {
                    canvasObj.ChangeGroup(SORTING_LAYERS.TUTOR,orderInLayer);
                }
                canvasObj.ChangeGroup(canvasObj.DesiredGroup, orderInLayer);
            }
            else if (TryResolve(obj, out IChangeSortGroupInSpriteRenderer spriteObj))
            {
                spriteObj.ChangeGroup(spriteObj.DesiredGroup, orderInLayer);
            }

            await UniTask.CompletedTask;
        }

        public static async UniTask ResetSortGroupInObj(this object obj)
        {
            if (TryResolve(obj, out IChangeSortGroupInCanvas canvasObj))
            {
                canvasObj.ResetGroup();
            }
            else if (TryResolve(obj, out IChangeSortGroupInSpriteRenderer spriteObj))
            {
                spriteObj.ResetGroup();
            }

            await UniTask.CompletedTask;
        }

        private static bool TryResolve<T>(object obj, out T result) where T : class
        {
            if (obj is GameObject go)
                return go.TryGetComponent(out result);

            result = obj as T;
            return result.IsNotNull();
        }
    }
}
