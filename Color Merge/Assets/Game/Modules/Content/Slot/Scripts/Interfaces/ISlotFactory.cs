using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Content.Slot.Scripts.Interfaces
{
    public interface ISlotFactory
    {
        UniTask WarmUpAsync(CancellationToken cancellationToken = default);
        SlotController Create(int id, Transform parent, ISlotDropHandler dropHandler);
        void Destroy(SlotController slotController);
    }
}
