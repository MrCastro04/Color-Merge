using System;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Контейнер улучшений игрока в текущем уровне: только хранит данные и сообщает об изменениях.
    // Пишут в него UpgradeGeneratorSystem (энергия, уровень генератора) и LevelController (лучший предмет на поле).
    public class PlayerUpgradesInLevel
    {
        public int GeneratorLevel { get; private set; } = 1;
        public int Energy { get; private set; }
        // Уровень лучшего предмета, который игрок собрал на поле (не тот, что выдаёт генератор).
        public int MaxItemLevel { get; private set; }
        // Сколько мерджей сделано на уровне (условие Make_Merge_X_Times).
        public int MergesCount { get; private set; }

        public event Action OnChanged;

        public void Reset()
        {
            GeneratorLevel = 1;
            Energy = 0;
            MaxItemLevel = 0;
            MergesCount = 0;
            OnChanged?.Invoke();
        }

        public void SetGeneratorLevel(int level)
        {
            GeneratorLevel = level;
            OnChanged?.Invoke();
        }

        public void AddEnergy(int amount)
        {
            Energy += amount;
            OnChanged?.Invoke();
        }

        public bool TrySpendEnergy(int amount)
        {
            if (Energy < amount)
                return false;

            Energy -= amount;
            OnChanged?.Invoke();
            return true;
        }

        public void ReportMerge()
        {
            MergesCount++;
            OnChanged?.Invoke();
        }

        // Только растёт.
        public void ReportItemLevel(int level)
        {
            if (level <= MaxItemLevel)
                return;

            MaxItemLevel = level;
            OnChanged?.Invoke();
        }
    }
}
