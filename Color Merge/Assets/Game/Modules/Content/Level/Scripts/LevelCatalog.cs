using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Порядок уровней: каждый уровень — отдельная сцена (должна быть в Build Settings).
    // Новый уровень = новая сцена с LevelScope + строка здесь, код не меняется.
    [CreateAssetMenu(menuName = "Game/Level/Level Catalog", fileName = "LevelCatalog")]
    public class LevelCatalog : ScriptableObject
    {
        [SerializeField] private List<string> _levelScenes = new();

        public int Count => _levelScenes.Count;

        public string GetSceneName(int levelIndex)
        {
            return _levelScenes[Mathf.Clamp(levelIndex, 0, _levelScenes.Count - 1)];
        }

        public bool HasLevel(int levelIndex) => levelIndex >= 0 && levelIndex < _levelScenes.Count;
    }
}
