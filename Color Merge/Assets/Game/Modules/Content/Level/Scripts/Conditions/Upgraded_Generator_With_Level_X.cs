using System;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    [Serializable]
    public class Upgraded_Generator_With_Level_X : LevelCondition
    {
        [Tooltip("Уровень предмета, который выдаёт генератор: 1 — стартовый")]
        [SerializeField, Min(2)] private int _generatorLevel = 3;

        public int GeneratorLevel => Mathf.Max(2, _generatorLevel);

        public override bool IsCompleted(PlayerUpgradesInLevel progress) => progress.GeneratorLevel >= GeneratorLevel;
    }
}
