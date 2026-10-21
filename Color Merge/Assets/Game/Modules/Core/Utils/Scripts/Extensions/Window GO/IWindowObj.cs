using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public interface IWindowObj
    {
        bool IsOpened { get; set; }
        float OpenDuration { get; }
        float CloseDuration { get; }
        GameObject ContainerContent { get; }
    }
}