using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using UnityEngine;

namespace Assets.Game.Modules.Content.Tutorial.Arrows
{
    public interface IMoveArrow : IMovator
    {
        ArrowDirectionType ArrowDirectionType { get; }
        Vector3 EndPoint { get; set; }
    }
}
