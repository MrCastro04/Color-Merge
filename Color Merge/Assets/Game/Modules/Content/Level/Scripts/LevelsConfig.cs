using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    [CreateAssetMenu(menuName = "Game/Level/Levels Config", fileName = "Levels")]
    public class LevelsConfig : ScriptableObject
    {
        [Tooltip("Порядок рівнів у меню та для кнопки CONTINUE")]
        [SerializeField] private List<LevelInfo> _levels = new();

        public int Count => _levels.Count;

        public LevelInfo Get(int index)
        {
            return _levels[index];
        }

        public bool TryGetByScene(string sceneName, out LevelInfo level)
        {
            int index = IndexOfScene(sceneName);

            if (index < 0)
            {
                level = null;
                return false;
            }

            level = _levels[index];
            return true;
        }

        public bool TryGetNext(string sceneName, out LevelInfo next)
        {
            int index = IndexOfScene(sceneName) + 1;

            if (index <= 0 || index >= _levels.Count)
            {
                next = null;
                return false;
            }

            next = _levels[index];
            return true;
        }

        private int IndexOfScene(string sceneName)
        {
            return _levels.FindIndex(level => level.SceneName == sceneName);
        }
    }

    [Serializable]
    public class LevelInfo
    {
        [Tooltip("Назва сцени рівня (як у Build Settings)")]
        [SerializeField] private string _sceneName;
        [Tooltip("Ключ збережень: чи пройдено рівень, найкращий час")]
        [SerializeField] private string _id;
        [Tooltip("Навчальний рівень: вікно NEW CONTENT у меню, рекорд часу не зберігається")]
        [SerializeField] private bool _isTutorial;

        public string SceneName => _sceneName;
        public string Id => _id;
        public bool IsTutorial => _isTutorial;
    }
}
