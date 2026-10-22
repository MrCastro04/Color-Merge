using System;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    public class PlayerUpgradesInLevel
    {
        public int GeneratorLevel { get; private set; } = 1;
        public int Energy { get; private set; }
        public int MaxItemLevel { get; private set; }
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

        public void ReportItemLevel(int level)
        {
            if (level <= MaxItemLevel)
                return;

            MaxItemLevel = level;
            OnChanged?.Invoke();
        }
    }
}
