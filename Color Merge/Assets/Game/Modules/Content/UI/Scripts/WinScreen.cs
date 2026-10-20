using System;
using Assets.Game.Modules.Core.Timer.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    public class WinScreen : UIScreen
    {
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private GameObject _newRecordLabel;
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

        public void SetResult(float time, float bestTime, bool isNewRecord)
        {
            _timeText.text = TimeFormatter.Format(time);
            _bestTimeText.text = $"BEST {TimeFormatter.Format(bestTime)}";

            if (_newRecordLabel != null)
                _newRecordLabel.SetActive(isNewRecord);
        }

        public void SetInteractable(bool isInteractable)
        {
            _restartButton.interactable = isInteractable;
            _menuButton.interactable = isInteractable;
        }

        // TODO: анимация появления (звёзды / конфетти из VFX)
    }
}
