using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Лёгкий список уровней для LifetimeScope: сами LevelConfig грузятся через IAssetProvider.
    [CreateAssetMenu(menuName = "Game/Level/Level Assets", fileName = "LevelAssets")]
    public class LevelAssets : ScriptableObject
    {
        [SerializeField] private List<LevelConfigReference> _levels = new();

        public int Count => _levels.Count;

        public AssetReferenceT<LevelConfig> GetLevel(int index)
        {
            return _levels[Mathf.Clamp(index, 0, _levels.Count - 1)];
        }
    }

    [Serializable]
    public class LevelConfigReference : AssetReferenceT<LevelConfig>
    {
        public LevelConfigReference(string guid) : base(guid)
        {
        }
    }
}
