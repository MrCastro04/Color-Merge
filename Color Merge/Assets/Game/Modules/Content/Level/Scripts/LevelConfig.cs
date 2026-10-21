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
        public const float MIN_TIME_LIMIT = 60f;
        public const float DEFAULT_TIME_LIMIT = 120f;

        [SerializeField] private string _id;
        [SerializeField] private ItemChainConfig _itemChain;

        [Header("Goal")]
        [SerializeField] private LevelMode _mode = LevelMode.COLLECT_ITEM;
        [Tooltip("Уровень предмета в ItemChain, который надо собрать: 1 — первый")]
        [SerializeField, Min(1)] private int _goalItemLevel = 7;
        [Tooltip("Только для TIMER: время на уровень, секунды (не меньше 60)")]
        [SerializeField, Min(MIN_TIME_LIMIT)] private float _timeLimit = DEFAULT_TIME_LIMIT;

        [Header("Board")]
        [SerializeField, Min(1)] private int _columns = 5;
        [SerializeField, Min(1)] private int _rows = 7;
        [SerializeField] private List<int> _blockedSlots = new();
        [SerializeField] private List<InitialItem> _initialItems = new();

        [Header("Generator")]
        [Tooltip("-1 — без ограничения")]
        [SerializeField] private int _generatorCharges = -1;
        [Tooltip("Показывать кнопку улучшения генератора (энергия за мерджи)")]
        [SerializeField] private bool _generatorUpgradeEnabled = true;

        [Header("Stars (секунды)")]
        [Tooltip("Прошёл быстрее — 3 звезды")]
        [SerializeField, Min(0)] private float _threeStarsTime = 90f;
        [Tooltip("Прошёл быстрее — 2 звезды, иначе 1")]
        [SerializeField, Min(0)] private float _twoStarsTime = 180f;

        public string ID => _id;
        public ItemChainConfig ItemChain => _itemChain;
        public int Columns => _columns;
        public int Rows => _rows;
        public int SlotsCount => _columns * _rows;
        public IReadOnlyList<int> BlockedSlots => _blockedSlots;
        public IReadOnlyList<InitialItem> InitialItems => _initialItems;
        public int GeneratorCharges => _generatorCharges;
        public bool HasGeneratorLimit => _generatorCharges >= 0;
        public bool IsGeneratorUpgradeEnabled => _generatorUpgradeEnabled;

        public LevelMode Mode => _mode;
        public bool IsTimerMode => _mode == LevelMode.TIMER;
        public float TimeLimit => Mathf.Max(MIN_TIME_LIMIT, _timeLimit);
        public int GoalItemLevel => Mathf.Clamp(_goalItemLevel, 1, _itemChain.Items.Count);
        public ItemConfig GoalItem => _itemChain.GetByLevel(GoalItemLevel);

        public int GetStars(float time)
        {
            if (time <= _threeStarsTime) return 3;
            if (time <= _twoStarsTime) return 2;
            return 1;
        }

        private void OnValidate()
        {
            _timeLimit = Mathf.Max(MIN_TIME_LIMIT, _timeLimit);

            if (_itemChain != null && _itemChain.Items.Count > 0)
                _goalItemLevel = Mathf.Clamp(_goalItemLevel, 1, _itemChain.Items.Count);
        }

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
