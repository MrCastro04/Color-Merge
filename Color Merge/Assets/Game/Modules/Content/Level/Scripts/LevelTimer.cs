using System;
using Assets.Game.Modules.Core.Timer.Scripts;
using UnityEngine;
using VContainer.Unity;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    public class LevelTimer : ITickable
    {
        private readonly Stopwatch _stopwatch = new();

        public float Elapsed => _stopwatch.Elapsed;
        public bool IsRunning => _stopwatch.IsRunning;

        public event Action<float> OnTimeChanged;

        public void Start() => _stopwatch.Start();
        public void Stop() => _stopwatch.Stop();

        public void Reset()
        {
            _stopwatch.Reset();
            OnTimeChanged?.Invoke(0f);
        }

        public void Tick()
        {
            if (!_stopwatch.IsRunning)
                return;

            _stopwatch.Tick(Time.deltaTime);
            OnTimeChanged?.Invoke(_stopwatch.Elapsed);
        }
    }
}
