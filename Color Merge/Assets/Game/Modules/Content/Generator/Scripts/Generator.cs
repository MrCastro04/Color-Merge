using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    // Кнопка-генератор: спавнит предмет 1-го уровня в случайную свободную клетку.
    public class Generator : IInitializable, IDisposable
    {
        private readonly BoardController _boardController;
        private readonly GeneratorButtonView _buttonView;

        private ItemConfig _itemToSpawn;
        private int _charges = -1;

        public int Charges => _charges;
        public bool HasCharges => _charges != 0;

        public event Action OnChargesEnded;
        // Нажали, но свободных клеток нет.
        public event Action OnSpawnFailed;

        public Generator(BoardController boardController, GeneratorButtonView buttonView)
        {
            _boardController = boardController;
            _buttonView = buttonView;
        }

        public void Initialize()
        {
            _buttonView.OnClicked += HandleOnClicked;
        }

        public void Dispose()
        {
            _buttonView.OnClicked -= HandleOnClicked;
        }

        // charges < 0 — без ограничения
        public void Setup(ItemConfig itemToSpawn, int charges)
        {
            _itemToSpawn = itemToSpawn;
            _charges = charges;

            _buttonView.SetCharges(_charges);
            _buttonView.SetInteractable(HasCharges);
        }

        public void SetInteractable(bool isInteractable)
        {
            _buttonView.SetInteractable(isInteractable && HasCharges);
        }

        private void HandleOnClicked()
        {
            if (_itemToSpawn == null || !HasCharges)
                return;

            // TODO: визуальный фидбек «доска заполнена» (тряска кнопки)
            if (!_boardController.TrySpawnAtRandomFreeSlot(_itemToSpawn))
            {
                OnSpawnFailed?.Invoke();
                return;
            }

            if (_charges < 0)
                return;

            _charges--;
            _buttonView.SetCharges(_charges);

            if (_charges == 0)
            {
                _buttonView.SetInteractable(false);
                OnChargesEnded?.Invoke();
            }
        }
    }
}
