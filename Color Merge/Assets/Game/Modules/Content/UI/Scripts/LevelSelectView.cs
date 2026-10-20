using System;
using Assets.Game.Modules.Core.UI.Scripts;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Заготовка: выбор уровня (кнопки Level_01..03 с рекордами).
    public class LevelSelectView : UIScreen
    {
        public event Action<int> OnLevelSelected;

        // TODO: создать кнопки по LevelCatalog.Count, показать рекорд из BestTimeRepository,
        // по клику — OnLevelSelected(index) → LevelLoader.LoadLevelAsync. Пока нигде не регистрируется — нет на сцене.
        protected void Invoke_OnLevelSelected(int levelIndex)
        {
            OnLevelSelected?.Invoke(levelIndex);
        }
    }
}
