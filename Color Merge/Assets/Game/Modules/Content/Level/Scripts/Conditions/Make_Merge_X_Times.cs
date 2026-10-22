using System;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts.Conditions
{
    [Serializable]
    public class Make_Merge_X_Times : LevelCondition
    {
        [Tooltip("Скільки мерджів потрібно зробити")]
        [SerializeField, Min(1)] private int _mergesCount = 10;

        public int MergesCount => Mathf.Max(1, _mergesCount);

        public override bool IsCompleted(PlayerUpgradesInLevel progress) => progress.MergesCount >= MergesCount;
    }
}
