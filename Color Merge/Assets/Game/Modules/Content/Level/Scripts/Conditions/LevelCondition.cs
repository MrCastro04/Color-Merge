using System;
using Assets.Game.Modules.Core.Utils.Scripts;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    // Условие уровня — элемент списка LevelConfig.Conditions ([SerializeReference]).
    // Уровень пройден, когда выполнены все условия списка (проверяет LevelConditionsChecker).
    [Serializable]
    public abstract class LevelCondition
    {
        // Прогресс игрока только растёт (лучший предмет, уровень генератора) — выполненное условие не «откатывается».
        public abstract bool IsCompleted(PlayerUpgradesInLevel progress);
    }

    // Условие с необязательным лимитом времени: не успел за _timeLimit секунд — поражение.
    // Лимит работает только на уровне с условием Timer (без него таймера у игрока нет).
    [Serializable]
    public abstract class TimedLevelCondition : LevelCondition
    {
        public const float MIN_TIME_LIMIT = 10f;

        [SerializeField] private bool _hasTimer;
        [Tooltip("Секунды с первого мерджа")]
        [SerializeField, ShowIf(nameof(_hasTimer)), Min(MIN_TIME_LIMIT)] private float _timeLimit = 120f;

        public bool HasTimer => _hasTimer;
        public float TimeLimit => Mathf.Max(MIN_TIME_LIMIT, _timeLimit);
    }
}
