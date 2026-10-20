namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Условие уровня.
    public enum LevelMode
    {
        // Собрать предмет уровня X.
        COLLECT_ITEM,
        // Собрать предмет уровня X за отведённое время, иначе — поражение.
        TIMER,
    }
}
