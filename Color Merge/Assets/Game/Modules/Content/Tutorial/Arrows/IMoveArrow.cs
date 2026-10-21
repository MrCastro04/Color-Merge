using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using UnityEngine;

namespace Assets.Game.Modules.Content.Tutorial.Arrows
{
    // В Combo-Dungeon лежал рядом с IMovator (ExMoveAnimator), здесь — в контенте: Core не знает про ArrowDirectionType.
    public interface IMoveArrow : IMovator
    {
        ArrowDirectionType ArrowDirectionType { get; }
        Vector3 EndPoint { get; set; }
    }
}
