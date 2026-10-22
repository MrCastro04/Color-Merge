namespace Assets.Game.Modules.Content.GUI.Scripts
{
    public readonly struct LevelButtonState
    {
        public readonly bool IsLocked;
        public readonly bool HasBestTime;
        public readonly float BestTime;

        public LevelButtonState(bool isLocked, bool hasBestTime, float bestTime)
        {
            IsLocked = isLocked;
            HasBestTime = hasBestTime;
            BestTime = bestTime;
        }
    }
}
