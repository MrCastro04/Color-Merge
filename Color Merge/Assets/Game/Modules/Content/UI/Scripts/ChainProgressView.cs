using System.Collections.Generic;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Панель над доской: цепочка предметов по порядку улучшения (до цели уровня включительно).
    // Достигнутые ступени яркие, ещё не собранные — полупрозрачные; новая ступень «подпрыгивает».
    // На тутор поднимается над затемнением (IChangeSortGroupInCanvas).
    public class ChainProgressView : MonoBehaviour, IChangeSortGroupInCanvas
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private Vector2 _iconSize = new(100f, 100f);
        [SerializeField, Range(0f, 1f)] private float _lockedAlpha = 0.3f;
        [SerializeField] private Canvas _sortingCanvas;

        private readonly List<Image> _icons = new();
        private int _reachedLevel;

        public string DesiredGroup => SORTING_LAYERS.TUTOR;
        public string StartGroup { get; set; } = SORTING_LAYERS.DEFAULT;
        public Canvas SortingCanvas => _sortingCanvas;

        // Первая и последняя (цель уровня) иконки цепочки — на них указывают стрелки тутора.
        public RectTransform FirstIcon => _icons.Count > 0 ? (RectTransform)_icons[0].transform : null;
        public RectTransform GoalIcon => _icons.Count > 0 ? (RectTransform)_icons[^1].transform : null;

        public void Setup(IReadOnlyList<ItemConfig> chain)
        {
            foreach (var icon in _icons)
            {
                if (icon != null)
                    Destroy(icon.gameObject);
            }

            _icons.Clear();
            _reachedLevel = 0;

            for (int i = 0; i < chain.Count; i++)
            {
                var go = new GameObject($"Level {i + 1}", typeof(RectTransform), typeof(Image));
                go.layer = gameObject.layer;

                var rt = (RectTransform)go.transform;
                rt.SetParent(_container, false);
                rt.sizeDelta = _iconSize;

                var image = go.GetComponent<Image>();
                image.sprite = chain[i].Icon;
                image.color = WithAlpha(chain[i].Color, _lockedAlpha);
                image.preserveAspect = true;
                image.raycastTarget = false;

                _icons.Add(image);
            }
        }

        // level — уровень лучшего предмета на доске (1 — первый). Прогресс только растёт.
        public void SetReachedLevel(int level)
        {
            level = Mathf.Clamp(level, 0, _icons.Count);

            if (level <= _reachedLevel)
                return;

            for (int i = _reachedLevel; i < level; i++)
            {
                var icon = _icons[i];
                icon.color = WithAlpha(icon.color, 1f);

                // Анимируем только при обновлении во время игры, а не стартовую расстановку.
                if (_reachedLevel > 0)
                {
                    icon.transform.DOKill(true);
                    icon.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 6, 0.5f).SetLink(icon.gameObject);
                }
            }

            _reachedLevel = level;
        }

        private static Color WithAlpha(Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}
