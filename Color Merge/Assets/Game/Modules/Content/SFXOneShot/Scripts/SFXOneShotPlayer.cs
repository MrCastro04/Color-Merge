using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Sound.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Content.SFXOneShot.Scripts
{
    // Грузит все одноразовые звуки из Addressables (WarmUpAsync) и проигрывает их по типу через ISoundService.
    public class SFXOneShotPlayer
    {
        private readonly ISoundService _soundService;
        private readonly IAssetProvider _assetProvider;
        private readonly SFXOneShotConfig _config;

        private readonly Dictionary<SFXOneShotType, (AudioClip Clip, SFXOneShotConfig.Entry Entry)> _sounds = new();

        public SFXOneShotPlayer(ISoundService soundService, IAssetProvider assetProvider, SFXOneShotConfig config)
        {
            _soundService = soundService;
            _assetProvider = assetProvider;
            _config = config;
        }

        public async UniTask WarmUpAsync(CancellationToken cancellationToken = default)
        {
            var entries = _config.Sounds.Where(x => x.Clip != null && x.Clip.RuntimeKeyIsValid()).ToArray();

            var clips = await UniTask.WhenAll(
                entries.Select(x => _assetProvider.LoadAsync<AudioClip>(x.Clip, cancellationToken)));

            for (int i = 0; i < entries.Length; i++)
                _sounds[entries[i].Type] = (clips[i], entries[i]);
        }

        public void Play(SFXOneShotType type)
        {
            if (!_sounds.TryGetValue(type, out var sound))
                return;

            _soundService.PlayOneShot(sound.Clip, sound.Entry.Volume, sound.Entry.RandomizePitch);
        }
    }
}
