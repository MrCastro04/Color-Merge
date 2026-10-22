namespace Assets.Game.Modules.Core.Vibration.Scripts
{
    public interface IVibrationService
    {
        // Включена ли вибрация (галочка в настройках, сохраняется между сессиями).
        bool IsEnabled { get; }

        void SetEnabled(bool isEnabled);

        // Короткий отклик (мердж). Выключена — ничего не делает.
        void VibrateShort();
    }
}
