namespace Assets.Game.Modules.Core.Infrastructure.Tutorial.Scripts
{
    // Какой тутор проигрывает уровень (выбирается в LevelScope на сцене уровня).
    public enum LevelTutorialType
    {
        NONE,
        // Level_0: мердж, цепочка, генератор — Tutorial_0_Controller.
        BASICS,
        // Level_1: таймер — Tutorial_1_Controller.
        TIMER,
        // Level_2: кнопка улучшения генератора — Tutorial_2_Controller.
        GENERATOR_UPGRADE,
        // Level_5: конфета Candy_Connector — Tutorial_3_Controller.
        CANDY,
        // Level_7: стены (заблокированные клетки) — Tutorial_4_Controller.
        WALL,
        // Level_9: конфетная бомба — Tutorial_5_Controller.
        CANDY_BOMB,
    }
}
