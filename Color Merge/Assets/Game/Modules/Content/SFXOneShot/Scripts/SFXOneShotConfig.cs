using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Content.SFXOneShot.Scripts
{
    // Перечень одноразовых звуков: тип → клип в Addressables + громкость и случайный питч.
    [CreateAssetMenu(menuName = "Game/Sound/SFX One Shot Config", fileName = "SFXOneShot")]
    public class SFXOneShotConfig : ScriptableObject
    {
        [SerializeField] private List<Entry> _sounds = new();

        public IReadOnlyList<Entry> Sounds => _sounds;

        [Serializable]
        public class Entry
        {
            public SFXOneShotType Type;
            public AssetReferenceT<AudioClip> Clip;
            [Range(0f, 1f)] public float Volume = 1f;
            public bool RandomizePitch = true;
        }
    }
}
