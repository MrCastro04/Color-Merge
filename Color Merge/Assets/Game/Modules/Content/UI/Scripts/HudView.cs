using Assets.Game.Modules.Core.Timer.Scripts;
using TMPro;
using UnityEngine;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Таймер и рекорд рядом с ним.
    public class HudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private TMP_Text _levelText;

        public void SetTime(float seconds)
        {
            _timeText.text = TimeFormatter.Format(seconds);
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
