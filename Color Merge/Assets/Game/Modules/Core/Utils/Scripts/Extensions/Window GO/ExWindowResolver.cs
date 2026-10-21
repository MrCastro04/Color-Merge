using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    // Глобальный резолвер: находит IWindowObj-компонент на GameObject и открывает/закрывает его как окно.
    public static class ExWindowResolver
    {
        public static async UniTask OpenObjAsWindow(this GameObject go, CancellationToken ct = default)
        {
            if (go.TryGetComponent(out IWindowObj windowObj))
                await windowObj.OpenAsWindow(ct);
        }

        public static async UniTask CloseObjAsWindow(this GameObject go, CancellationToken ct = default)
        {
            if (go.TryGetComponent(out IWindowObj windowObj))
                await windowObj.CloseAsWindow(ct);
        }
    }
}
