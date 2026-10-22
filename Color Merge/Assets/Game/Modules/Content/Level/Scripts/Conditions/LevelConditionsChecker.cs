using System;
using System.Collections.Generic;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    public class LevelConditionsChecker
    {
        private readonly PlayerUpgradesInLevel _progress;
        private readonly LevelTimer _levelTimer;

        private IReadOnlyCollection<LevelCondition> _conditions = Array.Empty<LevelCondition>();

        public bool HasTimer { get; private set; }
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

        public bool IsTimeUp()
        {
            return HasTimer && !IsAllConditionsCompleted && _levelTimer.Elapsed >= TimerSeconds;
        }

        public float GetRemainingTime()
        {
            if (HasTimer)
                return Math.Max(0f, TimerSeconds - _levelTimer.Elapsed);
            else
                return 0f;
        }
    }
}
