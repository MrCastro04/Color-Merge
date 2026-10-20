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

        public event Action OnRestartClicked;

        protected override void Awake()
        {
            base.Awake();

            _restartButton.onClick.AddListener(() => OnRestartClicked?.Invoke());
        }

        public void SetResult(float time, float bestTime, bool isNewRecord)
        {
            _timeText.text = TimeFormatter.Format(time);
            _bestTimeText.text = $"BEST {TimeFormatter.Format(bestTime)}";

            if (_newRecordLabel != null)
                _newRecordLabel.SetActive(isNewRecord);
        }

        // TODO: анимация появления (звёзды / конфетти из VFX)
    }
}
