using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Level.Scripts.Conditions;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.Serialization;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    [CreateAssetMenu(menuName = "Game/Level/Level Config", fileName = "Level_")]
    public class LevelConfig : SerializedScriptableObject
    {
        [SerializeField] private ItemChainConfig _itemChain;

        [Header("Conditions")]
        [OdinSerialize, NonSerialized, ShowInInspector] private HashSet<LevelCondition> _conditions = new();

        [Header("Board")]
        [SerializeField, Min(1)] private int _columns = 5;
        [SerializeField, Min(1)] private int _rows = 7;
        [SerializeField] private List<int> _blockedSlots = new();
        [SerializeField] private List<InitialItem> _initialItems = new();

        [Header("Candy Connector")]
        [Tooltip("Особливий предмет: предмет X + цукерка → предмет X+1")]
        [SerializeField] private CandyConnectorConfig _candyConnector;
        [Tooltip("Клітинки, де стоять цукерки на старті")]
        [SerializeField] private List<InitialCandy> _initialCandies = new();

        [Header("Candy Bomb")]
        [Tooltip("Задан — у игрока кнопка бомбы слева от генератора. Взрыв превращает предметы и стены в конфеты (нужен Candy Connector)")]
        [SerializeField] private CandyBombConfig _candyBomb;
        [Tooltip("Скільки енергії (1 мердж = 1 енергія) коштує одна бомба")]
        [SerializeField, Min(1)] private int _candyBombCost = 5;

        [Header("Generator")]
        [Tooltip("-1 — без обмеження")]
        [SerializeField] private int _generatorCharges = -1;
        [Tooltip("Показувати кнопку покращення генератора (енергія за мерджі)")]
        [SerializeField] private bool _generatorUpgradeEnabled = true;

        [Header("Stars (секунди, лише з умовою Timer)")]
        [Tooltip("Пройшов швидше — 3 зірки")]
        [SerializeField, Min(0)] private float _threeStarsTime = 90f;
        [Tooltip("Пройшов швидше — 2 зірки, інакше 1")]
        [SerializeField, Min(0)] private float _twoStarsTime = 180f;

        public ItemChainConfig ItemChain => _itemChain;
        public int Columns => _columns;
        public int Rows => _rows;
        public int SlotsCount => _columns * _rows;
        public IReadOnlyList<int> BlockedSlots => _blockedSlots;
        public IReadOnlyList<InitialItem> InitialItems => _initialItems;
        public CandyConnectorConfig CandyConnector => _candyConnector;
        public IReadOnlyList<InitialCandy> InitialCandies => _initialCandies;
        public CandyBombConfig CandyBomb => _candyBomb;
        public int CandyBombCost => Mathf.Max(1, _candyBombCost);
        public int GeneratorCharges => _generatorCharges;
        public bool HasGeneratorLimit => _generatorCharges >= 0;
        public bool IsGeneratorUpgradeEnabled => _generatorUpgradeEnabled;

        public IReadOnlyCollection<LevelCondition> Conditions => _conditions;
        public bool HasTimer => _conditions.Any(condition => condition is Conditions.Timer);

        public float TimerSeconds
        {
            get
            {
                var timer = _conditions.OfType<Conditions.Timer>().FirstOrDefault();

                if (timer != null)
                    return timer.Seconds;
                else
                    return 0f;
            }
        }

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

                if (level > 0)
                    return Mathf.Min(level, _itemChain.Items.Count);
                else
                    return _itemChain.Items.Count;
            }
        }

        public bool HasItemGoal => _conditions.Any(condition => condition is Get_Item_With_Level_X);

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

        public int GeneratorMaxLevel
        {
            get
            {
                int max;

                if (HasItemGoal)
                    max = TargetItemLevel - 1;
                else
                    max = _itemChain.Items.Count;

                foreach (var condition in _conditions)
                {
                    if (condition is Upgraded_Generator_With_Level_X generator)
                        max = Mathf.Max(max, generator.GeneratorLevel);
                }

                return Mathf.Clamp(max, 1, _itemChain.Items.Count);
            }
        }

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
            [Tooltip("Рівень предмета в ItemChain: 1 — перший")]
            [Min(1)] public int ItemStartLevel;
        }

        [Serializable]
        public struct InitialCandy
        {
            public int SlotID;
        }
    }
}
