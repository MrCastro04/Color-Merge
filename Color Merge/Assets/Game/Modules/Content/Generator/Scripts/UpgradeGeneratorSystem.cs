using System;
using Assets.Game.Modules.Content.Board.Scripts;
using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Level.Scripts;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    // Улучшение генератора за энергию.
    // Мердж двух предметов того же уровня, что сейчас выдаёт генератор, → +1 энергия.
    // ENERGY_PER_UPGRADE энергии → кнопка активна; нажали → генератор.Upgrade().
    public class UpgradeGeneratorSystem : IInitializable, IDisposable
    {
        public const int ENERGY_PER_UPGRADE = 3;

        private readonly Generator _generator;
        private readonly BoardController _boardController;
        private readonly PlayerUpgradesInLevel _upgrades;
        private readonly UpgradeButtonView _upgradeButtonView;

        private ItemChainConfig _chain;
        private bool _isInteractable = true;
        private bool _isEnabled = true;

        public event Action OnUpgraded;
        public event Action OnEnergyGained;

        public UpgradeGeneratorSystem(
            Generator generator, BoardController boardController,
            PlayerUpgradesInLevel upgrades, UpgradeButtonView upgradeButtonView)
        {
            _generator = generator;
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

        // Вызывает LevelController после Generator.Setup.
        // isEnabled = false — уровень без улучшений: кнопки нет, энергия не начисляется.
        public void Setup(ItemChainConfig chain, bool isEnabled)
        {
            _chain = chain;
            _isEnabled = isEnabled;
            _upgradeButtonView.SetVisible(isEnabled);
            _upgrades.SetGeneratorLevel(_generator.Level);
            UpdateButton();
        }

        public void SetInteractable(bool isInteractable)
        {
            _isInteractable = isInteractable;
            UpdateButton();
        }

        // merged — результат мерджа; исходные предметы были на уровень ниже.
        private void HandleOnItemMerged(ItemController merged)
        {
            if (_chain == null || !_isEnabled)
                return;

            int sourceLevel = _chain.GetLevel(merged.Config) - 1;

            if (sourceLevel != _generator.Level)
                return;

            _upgrades.AddEnergy(1);
            OnEnergyGained?.Invoke();
        }

        private void HandleOnUpgradeClicked()
        {
            if (!_isEnabled || !_isInteractable || !_generator.CanUpgrade)
                return;

            if (!_upgrades.TrySpendEnergy(ENERGY_PER_UPGRADE))
                return;

            _generator.Upgrade();
            _upgrades.SetGeneratorLevel(_generator.Level);

            OnUpgraded?.Invoke();
        }

        private void UpdateButton()
        {
            if (!_generator.CanUpgrade)
            {
                _upgradeButtonView.SetMaxed();
                return;
            }

            if (_isInteractable && _upgrades.Energy >= ENERGY_PER_UPGRADE)
                _upgradeButtonView.SetReady(_generator.Level, _generator.Level + 1);
            else
                _upgradeButtonView.SetNotReady(Math.Min(_upgrades.Energy, ENERGY_PER_UPGRADE), ENERGY_PER_UPGRADE);
        }
    }
}
