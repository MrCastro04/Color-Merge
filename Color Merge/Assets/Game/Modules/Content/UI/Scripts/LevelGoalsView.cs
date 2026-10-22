using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Значки условий победы сверху слева (обычные уровни): нужный предмет — его картинка,
    // прокачка генератора — выключенная кнопка улучшения и «Lvl N». Таймер и мерджи здесь не показываются.
    // Шаблоны лежат выключенными в _container (HorizontalLayoutGroup их не учитывает).
    public class LevelGoalsView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private Image _itemGoalTemplate;
        [SerializeField] private GameObject _generatorGoalTemplate;
        [SerializeField] private TMP_Text _generatorGoalLevelText;

        private readonly List<GameObject> _entries = new();

        public void Clear()
        {
            foreach (var entry in _entries)
            {
                if (entry != null)
                    Destroy(entry);
            }

            _entries.Clear();
            gameObject.SetActive(false);
        }

        public void AddItemGoal(Sprite icon)
        {
            var image = Instantiate(_itemGoalTemplate, _container);
            image.sprite = icon;
            Add(image.gameObject);
        }

        public void AddGeneratorGoal(int generatorLevel)
        {
            _generatorGoalLevelText.text = $"Lvl {generatorLevel}";
            Add(Instantiate(_generatorGoalTemplate, _container));
        }

        private void Add(GameObject entry)
        {
            entry.SetActive(true);
            _entries.Add(entry);
            gameObject.SetActive(true);
        }
    }
}
