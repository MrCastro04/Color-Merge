using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Item.Scripts.Interfaces;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Core.Vibration.Scripts;
using UnityEngine;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.CandyBomb.Scripts
{
    // Конфетная бомба (как UpgradeGeneratorSystem для генератора):
    // каждый мердж (и с конфетой) → +1 энергия; энергии хватает на цену уровня → кнопка активна;
    // нажали → на кнопке появляется бомба; перенесли на клетку → BoardController взрывает её,
    // здесь — эффект, вибрация и сброс кнопки. Бомбы в конфиге уровня нет — кнопка скрыта.
    public class CandyBombSystem : IInitializable, IDisposable
    {
        private readonly BoardController _boardController;
        private readonly CandyBombButtonView _buttonView;
        private readonly IItemFactory _itemFactory;
        private readonly ExplosionView _explosionView;
        private readonly IVibrationService _vibrationService;

        private CandyBombConfig _config;
        private ItemController _bomb;
        private bool _isInteractable = true;

        public bool IsEnabled { get; private set; }
        public int Energy { get; private set; }
        public int Cost { get; private set; }

        public event Action OnBombCreated;
        public event Action OnExploded;

        public CandyBombSystem(
            BoardController boardController, CandyBombButtonView buttonView, IItemFactory itemFactory,
            ExplosionView explosionView, IVibrationService vibrationService)
        {
            _boardController = boardController;
            _buttonView = buttonView;
            _itemFactory = itemFactory;
            _explosionView = explosionView;
            _vibrationService = vibrationService;
        }

        public void Initialize()
        {
            _boardController.OnItemMerged += HandleOnItemMerged;
            _boardController.OnBombExploded += HandleOnBombExploded;
            _buttonView.OnClicked += HandleOnButtonClicked;
        }

        public void Dispose()
        {
            _boardController.OnItemMerged -= HandleOnItemMerged;
            _boardController.OnBombExploded -= HandleOnBombExploded;
            _buttonView.OnClicked -= HandleOnButtonClicked;
        }

        // Вызывает LevelController при сборке уровня.
        public void Setup(LevelConfig levelConfig)
        {
            _config = levelConfig.CandyBomb;
            Cost = levelConfig.CandyBombCost;
            Energy = 0;
            IsEnabled = _config != null;

            _buttonView.SetVisible(IsEnabled);
            UpdateView();
        }

        // Тутор дарит заряд, чтобы игрок сразу попробовал бомбу.
        public void AddEnergy(int amount)
        {
            if (!IsEnabled)
                return;

            Energy = Mathf.Min(Energy + amount, Cost);
            UpdateView();
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;

            if (_bomb != null)
                _bomb.View.SetInteractable(isInteractable);

            UpdateView();
        }

        private void HandleOnItemMerged(ItemController merged) => AddEnergy(1);

        private void HandleOnButtonClicked()
        {
            if (!CanCreateBomb())
                return;

            Energy -= Cost;
            _bomb = _itemFactory.Create(_config, _buttonView.HolderRT);
            _buttonView.PlaceBomb(_bomb.View);
            _buttonView.SetReady(true);

            UpdateView();
            OnBombCreated?.Invoke();
        }

        private void HandleOnBombExploded(int centerId, CandyBombConfig config)
        {
            _bomb = null;
            _buttonView.SetReady(false);

            var slot = (RectTransform)_boardController.Slots[centerId].View.transform;
            float cellSize = slot.rect.width * slot.lossyScale.x;

            _explosionView.Play(slot.position, cellSize * (config.Radius + 0.5f));
            _vibrationService.VibrateStrong();

            UpdateView();
            OnExploded?.Invoke();
        }

        private bool CanCreateBomb() => IsEnabled && _isInteractable && _bomb == null && Energy >= Cost;

        private void UpdateView()
        {
            if (!IsEnabled)
                return;

            _buttonView.SetEnergy(Energy, Cost);
            _buttonView.SetInteractable(CanCreateBomb());
        }
    }
}
