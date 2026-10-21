using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public interface IFadeObj
    {
        float FadeInDuration { get; }
        float FadeOutDuration { get; }
        CanvasGroup CanvasGroup { get; }
    }
}
