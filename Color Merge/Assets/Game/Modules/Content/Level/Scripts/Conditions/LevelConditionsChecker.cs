using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    // Проверяет условия уровня из LevelConfig по прогрессу игрока (PlayerUpgradesInLevel) и таймеру уровня.
    // Когда выполнены все условия списка — один раз выставляет IsAllConditionsCompleted (уровень пройден).
    public class LevelConditionsChecker
    {
        private readonly PlayerUpgradesInLevel _progress;
        private readonly LevelTimer _levelTimer;

        private IReadOnlyList<LevelCondition> _conditions = Array.Empty<LevelCondition>();

        // В списке есть Timer: у игрока таймер, и работают лимиты времени условий.
        public bool HasTimer { get; private set; }
        public bool IsAllConditionsCompleted { get; private set; }

        public event Action OnAllConditionsCompleted;

        public LevelConditionsChecker(PlayerUpgradesInLevel progress, LevelTimer levelTimer)
        {
            _progress = progress;
            _levelTimer = levelTimer;
        }

        public void Setup(LevelConfig levelConfig)
        {
            _conditions = levelConfig.Conditions;
            HasTimer = levelConfig.HasTimer;
            IsAllConditionsCompleted = false;
        }

        public bool TryCompleteAll()
        {
            if (IsAllConditionsCompleted)
                return true;

            foreach (var condition in _conditions)
            {
                if (condition != null && !condition.IsCompleted(_progress))
                    return false;
            }

            IsAllConditionsCompleted = true;
            OnAllConditionsCompleted?.Invoke();
            return true;
        }

        // Не успел выполнить условие с лимитом. Без Timer в списке проиграть по времени нельзя.
        public bool IsTimeUp()
        {
            if (!HasTimer || IsAllConditionsCompleted)
                return false;

            foreach (var timed in GetPendingTimedConditions())
            {
                if (_levelTimer.Elapsed >= timed.TimeLimit)
                    return true;
            }

            return false;
        }

        // Сколько осталось до ближайшего лимита среди ещё не выполненных условий.
        public bool TryGetNearestDeadline(out float remaining)
        {
            remaining = float.MaxValue;
            bool hasDeadline = false;

            if (!HasTimer)
                return false;

            foreach (var timed in GetPendingTimedConditions())
            {
                remaining = Mathf.Min(remaining, timed.TimeLimit - _levelTimer.Elapsed);
                hasDeadline = true;
            }

            remaining = Mathf.Max(0f, remaining);
            return hasDeadline;
        }

        private IEnumerable<TimedLevelCondition> GetPendingTimedConditions()
        {
            foreach (var condition in _conditions)
            {
                if (condition is TimedLevelCondition { HasTimer: true } timed && !timed.IsCompleted(_progress))
                    yield return timed;
            }
        }
    }
}
