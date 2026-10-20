using UnityEngine;

namespace Assets.Game.Modules.Core.Timer.Scripts
{
    public static class TimeFormatter
    {
        // 83.456 → "01:23.45"
        public static string Format(float seconds)
        {
            seconds = Mathf.Max(0f, seconds);

            int minutes = (int)(seconds / 60f);
            int wholeSeconds = (int)(seconds % 60f);
            int hundredths = (int)((seconds - Mathf.Floor(seconds)) * 100f);

            return $"{minutes:00}:{wholeSeconds:00}.{hundredths:00}";
        }
    }
}
