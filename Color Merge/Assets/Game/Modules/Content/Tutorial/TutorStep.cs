using System;
using Assets.Game.Modules.Content.Tutorial.Arrows;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Tutorial
{
    public enum SkipCondition
    {
        Tap,
        ItemMerged,
        ButtonClick,
    }

    [Serializable]
    public struct TutorStep
    {
        public Tutor Tutor;
        public SkipCondition SkipCondition;
        public bool UseBackgroundFade;

        [Tooltip("Только для SkipCondition = ButtonClick")]
        public Button RequiredButton;
    }

    [Serializable]
    public struct ArrowSpawnInfo
    {
        public RectTransform StartPoint;
        public ArrowDirectionType Direction;
        public bool IsAnimated;
    }
}
