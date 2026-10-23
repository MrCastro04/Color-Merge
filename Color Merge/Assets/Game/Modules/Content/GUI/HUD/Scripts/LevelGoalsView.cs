using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Assets.Game.Modules.Content.GUI.HUD.Scripts
{
    public class LevelGoalsView : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
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
