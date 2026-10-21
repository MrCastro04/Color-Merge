using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.Utils.Scripts.Extensions
{
    public interface IPulseObject
    {
        Sequence PulseSequence { get; set; }

        public RectTransform RT{get;}
        Ease PulseEase { get; }
        float MaxPulseScale { get; }
        float MinPulseScale { get; }
        float PulseDuration { get; }
    }

    public interface IPulseObjWithPointer : IPulseObject
    {
        bool HasPointer { get; set; }
    }
}