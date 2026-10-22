namespace Assets.Game.Modules.Core.Timer.Scripts
{
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
