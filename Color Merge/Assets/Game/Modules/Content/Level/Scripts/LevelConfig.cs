using System;
using System.Collections.Generic;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Level.Scripts.Conditions;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Уровень целиком настраивается здесь, без изменения кода.
    // ID клетки = row * Columns + column, отсчёт с левого верхнего угла.
    // Условия прохождения — список _conditions (добавляются кнопкой в инспекторе, LevelConfigEditor).
    [CreateAssetMenu(menuName = "Game/Level/Level Config", fileName = "Level_")]
    public class LevelConfig : ScriptableObject
    {
        [SerializeField] private string _id;
        [SerializeField] private ItemChainConfig _itemChain;
        [Tooltip("Учебный уровень: рекорд времени не сохраняется и не показывается")]
        [SerializeField] private bool _isTutorial;

        [Header("Conditions")]
        [SerializeReference] private List<LevelCondition> _conditions = new();

        [Header("Board")]
        [SerializeField, Min(1)] private int _columns = 5;
        [SerializeField, Min(1)] private int _rows = 7;
        [SerializeField] private List<int> _blockedSlots = new();
        [SerializeField] private List<InitialItem> _initialItems = new();

        [Header("Candy Connector")]
        [Tooltip("Особый предмет: предмет X + конфета → предмет X+1")]
        [SerializeField] private CandyConnectorConfig _candyConnector;
        [Tooltip("Клетки, где стоят конфеты на старте")]
        [SerializeField] private List<InitialCandy> _initialCandies = new();

        [Header("Generator")]
        [Tooltip("-1 — без ограничения")]
        [SerializeField] private int _generatorCharges = -1;
        [Tooltip("Показывать кнопку улучшения генератора (энергия за мерджи)")]
        [SerializeField] private bool _generatorUpgradeEnabled = true;

        [Header("Stars (секунды, только с условием Timer)")]
        [Tooltip("Прошёл быстрее — 3 звезды")]
        [SerializeField, Min(0)] private float _threeStarsTime = 90f;
        [Tooltip("Прошёл быстрее — 2 звезды, иначе 1")]
        [SerializeField, Min(0)] private float _twoStarsTime = 180f;

        public string ID => _id;
        public ItemChainConfig ItemChain => _itemChain;
        public bool IsTutorial => _isTutorial;
        public int Columns => _columns;
        public int Rows => _rows;
        public int SlotsCount => _columns * _rows;
        public IReadOnlyList<int> BlockedSlots => _blockedSlots;
        public IReadOnlyList<InitialItem> InitialItems => _initialItems;
        public CandyConnectorConfig CandyConnector => _candyConnector;
        public IReadOnlyList<InitialCandy> InitialCandies => _initialCandies;
        public int GeneratorCharges => _generatorCharges;
        public bool HasGeneratorLimit => _generatorCharges >= 0;
        public bool IsGeneratorUpgradeEnabled => _generatorUpgradeEnabled;

        public IReadOnlyList<LevelCondition> Conditions => _conditions;
        public bool HasTimer => _conditions.Exists(condition => condition is Conditions.Timer);

        // Цель-предмет: самый высокий Get_Item_With_Level_X; нет такого условия — вся цепочка.
        public int TargetItemLevel
        {
            get
            {
                int level = 0;

                foreach (var condition in _conditions)
                {
                    if (condition is Get_Item_With_Level_X getItem)
                        level = Mathf.Max(level, getItem.ItemLevel);
                }

                return level > 0 ? Mathf.Min(level, _itemChain.Items.Count) : _itemChain.Items.Count;
            }
        }

        public bool HasItemGoal => _conditions.Exists(condition => condition is Get_Item_With_Level_X);

        // Сколько мерджей нужно (самое большое Make_Merge_X_Times); 0 — условия на мерджи нет.
        public int MergesTarget
        {
            get
            {
                int target = 0;

                foreach (var condition in _conditions)
                {
                    if (condition is Make_Merge_X_Times merges)
                        target = Mathf.Max(target, merges.MergesCount);
                }

                return target;
            }
        }

        // Генератор не выдаёт цель-предмет сам (до цели минус один),
        // но условие на прокачку генератора всегда достижимо.
        public int GeneratorMaxLevel
        {
            get
            {
                int max = HasItemGoal ? TargetItemLevel - 1 : _itemChain.Items.Count;

                foreach (var condition in _conditions)
                {
                    if (condition is Upgraded_Generator_With_Level_X generator)
                        max = Mathf.Max(max, generator.GeneratorLevel);
                }

                return Mathf.Clamp(max, 1, _itemChain.Items.Count);
            }
        }

        // Без таймера звёзды за время не считаются — всегда 3.
        public int GetStars(float time)
        {
            if (!HasTimer) return 3;
            if (time <= _threeStarsTime) return 3;
            if (time <= _twoStarsTime) return 2;
            return 1;
        }

        [Serializable]
        public struct InitialItem
        {
            public int SlotID;
            [FormerlySerializedAs("Level")]
            [Tooltip("Уровень предмета в ItemChain: 1 — первый")]
            [Min(1)] public int ItemStartLevel;
        }

        [Serializable]
        public struct InitialCandy
        {
            public int SlotID;
        }
    }
}
