using System;
using Assets.Game.Modules.Core.UI.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Экран поражения: ходов больше нет (генератор пуст или доска заполнена, и пар для мерджа нет).
    public class LoseScreen : UIScreen
    {
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _menuButton;

        public event Action OnRestartClicked;
        public event Action OnMenuClicked;

        protected override void Awake()
        {
            base.Awake();

            _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
            _menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
        }

        public void SetInteractable(bool isInteractable)
        {
            _restartButton.interactable = isInteractable;
            _menuButton.interactable = isInteractable;
        }
    }
}
