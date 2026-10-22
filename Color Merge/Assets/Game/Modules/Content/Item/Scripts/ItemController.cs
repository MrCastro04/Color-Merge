namespace Assets.Game.Modules.Content.Item.Scripts
{
    public class ItemController
    {
        public ItemConfig Config { get; }
        public ItemView View { get; }

        public ItemController(ItemConfig config, ItemView view)
        {
            Config = config;
            View = view;

            InitView();
        }

        public bool IsSameAs(ItemController other)
        {
            return other != null && other != this && other.Config == Config;
        }

        private void InitView()
        {
            View.Controller = this;
            View.Init();

            View.SetIcon(Config.Icon);
            View.SetColor(Config.Color);
        }
    }
}
