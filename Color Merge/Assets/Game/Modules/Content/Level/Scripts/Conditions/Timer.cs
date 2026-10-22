using System;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    // Есть в списке — у игрока таймер: обратный отсчёт от _seconds (виден сразу, идёт с первого мерджа),
    // дошёл до нуля — поражение. Плюс рекорд и звёзды за время; работают лимиты _hasTimer остальных условий.
    // Нет — таймера нет и проиграть по времени нельзя. Само по себе всегда выполнено.
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
