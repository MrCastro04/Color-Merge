using System;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    // Есть в списке — у игрока таймер (секундомер с первого мерджа, рекорд, звёзды за время)
    // и работают лимиты времени у остальных условий. Нет — таймера нет и проиграть по времени нельзя.
    // Само по себе всегда выполнено.
    [Serializable]
    public class Timer : LevelCondition
    {
        public override bool IsCompleted(PlayerUpgradesInLevel progress) => true;
    }
}
