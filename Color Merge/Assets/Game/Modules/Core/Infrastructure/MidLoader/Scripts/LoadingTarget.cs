namespace Assets.Game.Modules.Core.Infrastructure.MidLoader.Scripts
{
    public class LoadingTarget
    {
        public string SceneName { get; private set; }
        public bool ShowTips { get; private set; }
        public float HoldAfterLoad { get; private set; }

        public void Set(string sceneName, bool showTips, float holdAfterLoad)
        {
            SceneName = sceneName;
            ShowTips = showTips;
            HoldAfterLoad = holdAfterLoad;
        }
    }
}
