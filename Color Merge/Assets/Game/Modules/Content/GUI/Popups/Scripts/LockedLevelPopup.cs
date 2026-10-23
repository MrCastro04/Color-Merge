using System;
using Assets.Game.Modules.Core.UI.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.GUI.Popups.Scripts
{
    public class LockedLevelPopup : UIScreen
    {
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _okButton;

        private bool _isListening;

        public event Action OnOkClicked;

        public void SetLevel(int previousLevelNumber)
        {
            _messageText.text = $"Complete level {previousLevelNumber}\nto unlock it!";

            if (_isListening)
                return;

            _okButton.onClick.AddListener(() => OnOkClicked?.Invoke());
            _isListening = true;
        }
    }
}
