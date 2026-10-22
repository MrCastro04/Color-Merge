using System;
using Assets.Game.Modules.Core.UI.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Окно перед учебным уровнем (0–2): в уровне новый контент и обучение. PLAY — грузим уровень, крестик — назад к выбору.
    public class TutorialLevelPopup : UIScreen
    {
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _playButton;
        [SerializeField] private Button _closeButton;

        private bool _isListening;

        public int LevelIndex { get; private set; }

        public event Action OnPlayClicked;
        public event Action OnCloseClicked;

        public void SetLevel(int levelIndex)
        {
            LevelIndex = levelIndex;
            _messageText.text = $"Level {levelIndex} has new content.\nIt will teach you how to play!";

            if (_isListening)
                return;

            _playButton.onClick.AddListener(() => OnPlayClicked?.Invoke());
            _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
            _isListening = true;
        }

        public void SetInteractable(bool isInteractable)
        {
            _playButton.interactable = isInteractable;
            _closeButton.interactable = isInteractable;
        }
    }
}
