using Assets.Game.Modules.Core.Timer.Scripts;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Таймер и рекорд (только на уровне с условием Timer).
    // Есть невыполненное условие с лимитом — показывается оставшееся время; последние секунды — красным.
    public class HudView : MonoBehaviour
    {
        private const float WARNING_TIME = 10f;

        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private Color _warningColor = new(1f, 0.35f, 0.35f, 1f);

        private Color _timeColor;
        private bool _isColorCached;
        private bool _isWarning;

        // Уровень без условия Timer — ни таймера, ни рекорда.
        public void SetTimerVisible(bool isVisible)
        {
            _timeText.gameObject.SetActive(isVisible);

            if (!isVisible)
                _bestTimeText.gameObject.SetActive(false);
        }

        // Секундомер.
        public void SetTime(float seconds)
        {
            _timeText.text = TimeFormatter.Format(seconds);
            SetWarning(false);
        }

        // Обратный отсчёт: меньше WARNING_TIME — текст краснеет и пульсирует.
        public void SetRemainingTime(float seconds)
        {
            _timeText.text = TimeFormatter.Format(seconds);
            SetWarning(seconds <= WARNING_TIME);
        }

        private void SetWarning(bool isWarning)
        {
            if (!_isColorCached)
            {
                _timeColor = _timeText.color;
                _isColorCached = true;
            }

            if (isWarning == _isWarning)
                return;

            _isWarning = isWarning;
            _timeText.color = isWarning ? _warningColor : _timeColor;

            if (isWarning)
                _timeText.transform.DOPunchScale(Vector3.one * 0.15f, 0.4f, 4, 0.5f).SetLink(gameObject);
        }

        // Рекорд показывается только при повторном прохождении.
        public void SetBestTime(bool hasBestTime, float seconds)
        {
            _bestTimeText.gameObject.SetActive(hasBestTime);

            if (hasBestTime)
                _bestTimeText.text = $"BEST {TimeFormatter.Format(seconds)}";
        }

        public void SetLevelName(string levelName)
        {
            if (_levelText != null)
                _levelText.text = levelName;
        }

        // TODO: анимация нового рекорда (пульс _bestTimeText)
    }
}
