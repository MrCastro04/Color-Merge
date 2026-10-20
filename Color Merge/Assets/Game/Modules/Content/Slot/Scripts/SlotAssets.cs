using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Content.Slot.Scripts
{
    [CreateAssetMenu(menuName = "Game/Slot/Slot Assets", fileName = "SlotAssets")]
    public class SlotAssets : ScriptableObject
    {
        [SerializeField] private AssetReferenceGameObject _slotPrefab;

        public AssetReferenceGameObject SlotPrefab => _slotPrefab;
    }
}
