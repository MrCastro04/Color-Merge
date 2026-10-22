using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.GUI.Scripts
{
    public class SettingsButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;

        public event Action OnClicked;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleOnClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnClicked);
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        private void HandleOnClicked() => OnClicked?.Invoke();
    }
}
