using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    // Кнопка-генератор: спавнит предмет своего уровня в случайную свободную клетку.
    // Наружу отдаёт только свой уровень; улучшается через Upgrade() — его вызывает UpgradeGeneratorSystem.
    public class Generator : IInitializable, IDisposable
    {
        private readonly BoardController _boardController;
        private readonly GeneratorButtonView _buttonView;

        private ItemChainConfig _chain;
        private int _maxLevel;
        private int _charges = -1;

        public int Level { get; private set; } = 1;
        public bool CanUpgrade => Level < _maxLevel;
        public int Charges => _charges;
        public bool HasCharges => _charges != 0;

        private ItemConfig ItemToSpawn => _chain?.GetByLevel(Level);

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

        // maxLevel — выше этого уровня генератор не улучшается (чтобы не выдавать цель уровня напрямую).
        // charges < 0 — без ограничения.
        public void Setup(ItemChainConfig chain, int maxLevel, int charges)
        {
            _chain = chain;
            _maxLevel = Math.Max(1, maxLevel);
            _charges = charges;
            Level = 1;

            _buttonView.SetCharges(_charges);
            _buttonView.SetInteractable(HasCharges);
            UpdateView();
        }

        public void Upgrade()
        {
            if (!CanUpgrade)
                return;

            Level++;
            UpdateView();
            _buttonView.PlayUpgrade();
        }

        public void SetCentered(bool isCentered)
        {
            _buttonView.SetCentered(isCentered);
        }

        public void SetInteractable(bool isInteractable)
        {
            _buttonView.SetInteractable(isInteractable && HasCharges);
        }

        private void UpdateView()
        {
            _buttonView.SetLevel(Level);
            _buttonView.SetIcon(ItemToSpawn.Icon);
        }

        private void HandleOnClicked()
        {
            if (_chain == null || !HasCharges)
                return;

            // TODO: визуальный фидбек «доска заполнена» (тряска кнопки)
            if (!_boardController.TrySpawnAtRandomFreeSlot(ItemToSpawn))
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
