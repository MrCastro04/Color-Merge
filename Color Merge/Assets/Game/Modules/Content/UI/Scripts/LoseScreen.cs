using System;
using Assets.Game.Modules.Core.UI.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Экран поражения. Причина задаётся снаружи: ходов нет (OUT OF MOVES) или время вышло (TIME'S UP).
    public class LoseScreen : UIScreen
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
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

        public void SetReason(string title, string message)
        {
            if (_titleText != null)
                _titleText.text = title;

            if (_messageText != null)
                _messageText.text = message;
        }

        public void SetInteractable(bool isInteractable)
        {
            _restartButton.interactable = isInteractable;
            _menuButton.interactable = isInteractable;
        }
    }
}
