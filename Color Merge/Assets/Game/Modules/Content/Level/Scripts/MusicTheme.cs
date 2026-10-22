using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    [CreateAssetMenu(menuName = "Game/Sound/Music Theme", fileName = "MusicTheme")]
    public class MusicTheme : ScriptableObject
    {
        [SerializeField] private List<Entry> _music = new();

        public bool TryGet(MusicType type, out Entry entry)
        {
            entry = _music.Find(x => x.Type == type);
            return entry != null && entry.Clip != null && entry.Clip.RuntimeKeyIsValid();
        }

        [Serializable]
        public class Entry
        {
            public MusicType Type;
            public AssetReferenceT<AudioClip> Clip;
            public bool Loop = true;
        }
    }
}
