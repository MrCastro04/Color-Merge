using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Board.Scripts
{
    // Сетка клеток. Размер клетки подгоняется под доступную область (9:16, 9:19.5, 3:4):
    // клетка квадратная, берётся минимум из ширины и высоты на одну клетку.
    [RequireComponent(typeof(RectTransform))]
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private RectTransform _slotsContainer;
        [SerializeField] private GridLayoutGroup _grid;
        [SerializeField] private float _spacing = 8f;

        private int _columns;
        private int _rows;

        public RectTransform SlotsContainer => _slotsContainer;

        public void Init(int columns, int rows)
        {
            _columns = columns;
            _rows = rows;

            _grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            _grid.constraintCount = columns;
            _grid.spacing = new Vector2(_spacing, _spacing);
            _grid.childAlignment = TextAnchor.MiddleCenter;

            FitCells();
        }

        // Вызывать после создания клеток: GridLayoutGroup расставляет детей только на ребилде лейаута,
        // а HolderView.Set берёт размер холдера сразу.
        public void RebuildLayout()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(_slotsContainer);
        }

        private void OnRectTransformDimensionsChange()
        {
            FitCells();
        }

        private void FitCells()
        {
            if (_columns <= 0 || _rows <= 0 || _grid == null || _slotsContainer == null)
                return;

            var size = _slotsContainer.rect.size;
            var padding = _grid.padding;

            float availableWidth = size.x - padding.horizontal - _spacing * (_columns - 1);
            float availableHeight = size.y - padding.vertical - _spacing * (_rows - 1);

            float cellSize = Mathf.Floor(Mathf.Min(availableWidth / _columns, availableHeight / _rows));

            if (cellSize <= 0f)
                return;

            _grid.cellSize = new Vector2(cellSize, cellSize);
        }
    }
}
