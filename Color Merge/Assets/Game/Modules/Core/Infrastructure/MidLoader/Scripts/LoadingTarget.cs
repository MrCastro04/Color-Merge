namespace Assets.Game.Modules.Core.Infrastructure.MidLoader.Scripts
{
    // Куда ведёт текущий переход через Mid Loader (аналог GameProggres.NextLevelIndex в Combo-Dungeon).
    // Singleton в ApplicationScope: LevelLoader записывает, MidLoaderController читает.
    public class LoadingTarget
    {
        public string SceneName { get; private set; }
        public bool ShowTips { get; private set; }
        // Пауза после полного бара перед показом сцены (секунды).
        public float HoldAfterLoad { get; private set; }

        public void Set(string sceneName, bool showTips, float holdAfterLoad)
        {
            SceneName = sceneName;
            ShowTips = showTips;
            HoldAfterLoad = holdAfterLoad;
        }
    }
}
