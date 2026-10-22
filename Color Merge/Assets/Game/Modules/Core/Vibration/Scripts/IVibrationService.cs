namespace Assets.Game.Modules.Core.Vibration.Scripts
{
    public interface IVibrationService
    {
        bool IsEnabled { get; }

        void SetEnabled(bool isEnabled);

        void VibrateShort();

        void VibrateStrong();
    }
}
