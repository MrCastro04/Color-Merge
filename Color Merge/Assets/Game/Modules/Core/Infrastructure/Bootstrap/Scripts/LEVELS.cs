using System;

namespace Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts
{
    // Порядок уровней игры: сцена + ID (LevelConfig.ID) для рекорда и прогресса + учебный ли уровень.
    // Учебные: перед загрузкой из меню — окно «новый контент», рекорд не показывается.
    // Нужен меню (кнопки выбора) и экрану победы (CONTINUE → следующий уровень).
    public static class LEVELS
    {
        public static readonly (string Scene, string LevelId, bool IsTutorial)[] ALL =
        {
            (SCENE_NAMES.LEVEL_0, "level_00", true),
            (SCENE_NAMES.LEVEL_1, "level_01", true),
            (SCENE_NAMES.LEVEL_2, "level_02", true),
            (SCENE_NAMES.LEVEL_3, "level_03", false),
            (SCENE_NAMES.LEVEL_4, "level_04", false),
            (SCENE_NAMES.LEVEL_5, "level_05", true),
            (SCENE_NAMES.LEVEL_6, "level_06", false),
            (SCENE_NAMES.LEVEL_7, "level_07", true),
            (SCENE_NAMES.LEVEL_8, "level_08", false),
            (SCENE_NAMES.LEVEL_9, "level_09", true),
            (SCENE_NAMES.LEVEL_10, "level_10", false),
        };

        // -1 — сцена не уровень.
        public static int IndexOfScene(string scene) => Array.FindIndex(ALL, level => level.Scene == scene);
    }
}
