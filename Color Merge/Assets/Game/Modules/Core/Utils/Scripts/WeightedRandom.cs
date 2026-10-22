using System;
using System.Collections.Generic;

namespace Assets.Game.Modules.Core.Utils.Scripts
{
    public static class WeightedRandom
    {
        public static T Pick<T>(IReadOnlyList<T> items, Func<T, float> getWeight)
        {
            if (items == null || items.Count == 0)
                throw new ArgumentException("Список для WeightedRandom пуст.");

            float totalWeight = 0f;

            foreach (var item in items)
                totalWeight += Math.Max(0f, getWeight(item));

            if (totalWeight <= 0f)
                return items[UnityEngine.Random.Range(0, items.Count)];

            float roll = UnityEngine.Random.value * totalWeight;

            foreach (var item in items)
            {
                roll -= Math.Max(0f, getWeight(item));

                if (roll <= 0f)
                    return item;
            }

            return items[items.Count - 1];
        }
    }
}
