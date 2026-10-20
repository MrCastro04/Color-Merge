using System;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Ссылка на LevelConfig для LevelScope: сам конфиг грузится через IAssetProvider.
    [Serializable]
    public class LevelConfigReference : AssetReferenceT<LevelConfig>
    {
        public LevelConfigReference(string guid) : base(guid)
        {
        }
    }
}
