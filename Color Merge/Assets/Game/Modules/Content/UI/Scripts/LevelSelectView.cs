using System;
using Assets.Game.Modules.Core.UI.Scripts;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Заготовка: выбор уровня (кнопки Level_01..03 с рекордами).
    public class LevelSelectView : UIScreen
    {
        public event Action<int> OnLevelSelected;

        // TODO: кнопки выбора уровня с рекордом из BestTimeRepository, по клику — OnLevelSelected(index).
        // Пока нигде не регистрируется — нет на сцене.
        protected void Invoke_OnLevelSelected(int levelIndex)
        {
            OnLevelSelected?.Invoke(levelIndex);
        }
    }
}
