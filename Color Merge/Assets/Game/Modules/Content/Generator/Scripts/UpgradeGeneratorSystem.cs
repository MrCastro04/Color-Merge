using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Level.Scripts;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    public class UpgradeGeneratorSystem : IInitializable, IDisposable
    {
        public const int ENERGY_PER_UPGRADE = 3;

        private readonly GeneratorController _generatorController;
        private readonly BoardController _boardController;
        private readonly PlayerUpgradesInLevel _upgrades;
        private readonly UpgradeButtonView _upgradeButtonView;

        private ItemChainConfig _chain;
        private bool _isInteractable = true;
        private bool _isEnabled = true;

        public event Action OnUpgraded;
        public event Action OnEnergyGained;

        public UpgradeGeneratorSystem(
            GeneratorController generatorController, BoardController boardController,
            PlayerUpgradesInLevel upgrades, UpgradeButtonView upgradeButtonView)
        {
            _generatorController = generatorController;
            _boardController = boardController;
            _upgrades = upgrades;
            _upgradeButtonView = upgradeButtonView;
        }

        public void Initialize()
        {
            _boardController.OnItemMerged += HandleOnItemMerged;
            _upgradeButtonView.OnClicked += HandleOnUpgradeClicked;
            _upgrades.OnChanged += UpdateButton;
        }

        public void Dispose()
        {
            _boardController.OnItemMerged -= HandleOnItemMerged;
            _upgradeButtonView.OnClicked -= HandleOnUpgradeClicked;
            _upgrades.OnChanged -= UpdateButton;
        }

        public void Setup(ItemChainConfig chain, bool isEnabled)
        {
            _chain = chain;
            _isEnabled = isEnabled;
            _upgradeButtonView.SetVisible(isEnabled);
            _upgrades.SetGeneratorLevel(_generatorController.Level);
            UpdateButton();
        }

        public void SetShift(float shiftX)
        {
            _upgradeButtonView.SetShift(shiftX);
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;
            UpdateButton();
        }

        private void HandleOnItemMerged(ItemController merged)
        {
            if (_chain == null || !_isEnabled)
                return;

            int sourceLevel = _chain.GetLevel(merged.Config) - 1;

            if (sourceLevel != _generatorController.Level)
                return;

            _upgrades.AddEnergy(1);
            OnEnergyGained?.Invoke();
        }

        private void HandleOnUpgradeClicked()
        {
            if (!_isEnabled || !_isInteractable || !_generatorController.CanUpgrade)
                return;

            if (!_upgrades.TrySpendEnergy(ENERGY_PER_UPGRADE))
                return;

            _generatorController.Upgrade();
            _upgrades.SetGeneratorLevel(_generatorController.Level);

            OnUpgraded?.Invoke();
        }

        private void UpdateButton()
        {
            if (!_generatorController.CanUpgrade)
            {
                _upgradeButtonView.SetMaxed();
                return;
            }

            if (_isInteractable && _upgrades.Energy >= ENERGY_PER_UPGRADE)
                _upgradeButtonView.SetReady(_generatorController.Level, _generatorController.Level + 1);
            else
                _upgradeButtonView.SetNotReady(Math.Min(_upgrades.Energy, ENERGY_PER_UPGRADE), ENERGY_PER_UPGRADE);
        }
    }
}
