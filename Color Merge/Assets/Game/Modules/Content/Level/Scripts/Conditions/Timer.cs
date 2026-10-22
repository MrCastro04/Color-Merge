using System;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    [Serializable]
    public class Timer : LevelCondition
    {
        public const float MIN_SECONDS = 10f;

        [Tooltip("Время на уровень, секунды")]
        [SerializeField, Min(MIN_SECONDS)] private float _seconds = 120f;

        public float Seconds => Mathf.Max(MIN_SECONDS, _seconds);

        public override bool IsCompleted(PlayerUpgradesInLevel progress) => true;
    }
}
