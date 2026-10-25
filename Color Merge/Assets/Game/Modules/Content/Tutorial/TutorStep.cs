using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Tutorial
{
    public enum SkipCondition
    {
        Tap,
        ItemMerged,
        ButtonClick,
        External,
    }

    [Serializable]
    public struct TutorStep
    {
        public Tutor Tutor;
        public SkipCondition SkipCondition;
        public bool UseBackgroundFade;

        [Tooltip("Лише для SkipCondition = ButtonClick")]
        public Button RequiredButton;
    }
}
