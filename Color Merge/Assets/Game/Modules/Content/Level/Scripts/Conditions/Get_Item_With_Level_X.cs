using System;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    // Собрать на поле предмет уровня X (1 — первый в ItemChain).
    [Serializable]
    public class Get_Item_With_Level_X : TimedLevelCondition
    {
        [Tooltip("Уровень предмета в ItemChain: 1 — первый")]
        [SerializeField, Min(1)] private int _itemLevel = 7;

        public int ItemLevel => Mathf.Max(1, _itemLevel);

        public override bool IsCompleted(PlayerUpgradesInLevel progress) => progress.MaxItemLevel >= ItemLevel;
    }
}
