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
        // Время на уровень из условия Timer — общий обратный отсчёт.
        public float TimerSeconds { get; private set; }
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
            TimerSeconds = levelConfig.TimerSeconds;
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

        // Кончилось время уровня (Timer) или не успел выполнить условие с лимитом. Без Timer проиграть по времени нельзя.
        public bool IsTimeUp()
        {
            if (!HasTimer || IsAllConditionsCompleted)
                return false;

            if (_levelTimer.Elapsed >= TimerSeconds)
                return true;

            foreach (var timed in GetPendingTimedConditions())
            {
                if (_levelTimer.Elapsed >= timed.TimeLimit)
                    return true;
            }

            return false;
        }

        // Сколько осталось: время уровня (Timer) или ближайший лимит невыполненного условия — что раньше.
        public bool TryGetNearestDeadline(out float remaining)
        {
            remaining = float.MaxValue;

            if (!HasTimer)
                return false;

            remaining = TimerSeconds - _levelTimer.Elapsed;
            bool hasDeadline = true;

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
