using System;
using System.Collections.Generic;
using Assets.Game.Modules.Content.Item.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Уровень целиком настраивается здесь, без изменения кода.
    // ID клетки = row * Columns + column, отсчёт с левого верхнего угла.
    [CreateAssetMenu(menuName = "Game/Level/Level Config", fileName = "Level_")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private ItemChainConfig _itemChain;

        [Header("Board")]
        [SerializeField, Min(1)] private int _columns = 5;
        [SerializeField, Min(1)] private int _rows = 7;
        [SerializeField] private List<int> _blockedSlots = new();
        [SerializeField] private List<InitialItem> _initialItems = new();

        [Header("Generator")]
        [Tooltip("-1 — без ограничения")]
        [SerializeField] private int _generatorCharges = -1;

        public string ID => _id;
        public ItemChainConfig ItemChain => _itemChain;
        public int Columns => _columns;
        public int Rows => _rows;
        public int SlotsCount => _columns * _rows;
        public IReadOnlyList<int> BlockedSlots => _blockedSlots;
        public IReadOnlyList<InitialItem> InitialItems => _initialItems;
        public int GeneratorCharges => _generatorCharges;
        public bool HasGeneratorLimit => _generatorCharges >= 0;

        [Serializable]
        public struct InitialItem
        {
            public int SlotID;
            [FormerlySerializedAs("Level")]
            [Tooltip("Уровень предмета в ItemChain: 1 — первый")]
            [Min(1)] public int ItemStartLevel;
        }
    }
}
