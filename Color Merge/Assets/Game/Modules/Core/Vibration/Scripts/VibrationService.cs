using Assets.Game.Modules.Core.Save.Scripts;
using UnityEngine;

namespace Assets.Game.Modules.Core.Vibration.Scripts
{
    // Вибрация телефона. Handheld.Vibrate слишком длинная (~0.5 с), поэтому на Android короткий импульс
    // через системный Vibrator (VibrationEffect с API 26). В редакторе и на других платформах — ничего.
    // Ссылка на Handheld.Vibrate (запасной путь) заодно добавляет в манифест разрешение VIBRATE.
    public class VibrationService : IVibrationService
    {
        private const string ENABLED_KEY = "vibration_enabled";
        private const long SHORT_DURATION_MS = 35;
        private const int DEFAULT_AMPLITUDE = -1;
        private const int VIBRATION_EFFECT_MIN_SDK = 26;

        private readonly ISaveService _saveService;

#if UNITY_ANDROID && !UNITY_EDITOR
        private AndroidJavaObject _vibrator;
#endif

        public bool IsEnabled { get; private set; }

        public VibrationService(ISaveService saveService)
        {
            _saveService = saveService;
            IsEnabled = _saveService.GetInt(ENABLED_KEY, 1) == 1;
        }

        public void SetEnabled(bool isEnabled)
        {
            IsEnabled = isEnabled;
            _saveService.SetInt(ENABLED_KEY, isEnabled ? 1 : 0);
            _saveService.Save();
        }

        public void VibrateShort()
        {
            if (!IsEnabled)
                return;

#if UNITY_ANDROID && !UNITY_EDITOR
            try
            {
                VibrateAndroid(SHORT_DURATION_MS);
            }
            catch (System.Exception)
            {
                Handheld.Vibrate();
            }
#elif UNITY_IOS && !UNITY_EDITOR
            Handheld.Vibrate();
#endif
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private void VibrateAndroid(long milliseconds)
        {
            if (_vibrator == null)
            {
                using var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                using var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                _vibrator = activity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }

            using var version = new AndroidJavaClass("android.os.Build$VERSION");

            if (version.GetStatic<int>("SDK_INT") >= VIBRATION_EFFECT_MIN_SDK)
            {
                using var effectClass = new AndroidJavaClass("android.os.VibrationEffect");
                using var effect =
 effectClass.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, DEFAULT_AMPLITUDE);
                _vibrator.Call("vibrate", effect);
            }
            else
            {
                _vibrator.Call("vibrate", milliseconds);
            }
        }
#endif
    }
}