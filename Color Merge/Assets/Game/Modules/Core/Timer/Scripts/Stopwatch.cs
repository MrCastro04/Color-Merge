namespace Assets.Game.Modules.Core.Timer.Scripts
{
    // Секундомер без привязки к MonoBehaviour: время двигает тот, кто вызывает Tick.
    public class Stopwatch
    {
        public float Elapsed { get; private set; }
        public bool IsRunning { get; private set; }

        public void Start() => IsRunning = true;
        public void Stop() => IsRunning = false;

        public void Reset()
        {
            IsRunning = false;
            Elapsed = 0f;
        }

        public void Tick(float deltaTime)
        {
            if (!IsRunning)
                return;

            Elapsed += deltaTime;
        }
    }
}
