using System;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    [Serializable]
    public class LevelConfigReference : AssetReferenceT<LevelConfig>
    {
        public LevelConfigReference(string guid) : base(guid)
        {
        }
    }
}
