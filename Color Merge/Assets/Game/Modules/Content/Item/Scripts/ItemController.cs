namespace Assets.Game.Modules.Content.Item.Scripts
{
    // Аналог CardPresenter из Combo-Dungeon: связывает конфиг с вьюхой и настраивает её.
    // В контейнере не регистрируется — создаёт ItemFactory.
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
