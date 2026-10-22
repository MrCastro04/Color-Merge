using System;
using Assets.Game.Modules.Core.UI.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    public class MessagePopup : UIScreen
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _messageText;
        [SerializeField] private Button _okButton;

        public event Action OnOkClicked;

        protected override void Awake()
        {
            base.Awake();

            _okButton.onClick.AddListener(() => OnOkClicked?.Invoke());
        }

        public void SetMessage(string title, string message)
        {
            _titleText.text = title;
            _messageText.text = message;
        }
    }
}
