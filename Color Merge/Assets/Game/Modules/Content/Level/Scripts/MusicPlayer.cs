using System.Threading;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Sound.Scripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.Game.Modules.Content.Level.Scripts
{
    // Запуск музыки по типу из MusicTheme: грузит клип через IAssetProvider (кэшируется) и отдаёт в ISoundService.
    public class MusicPlayer
    {
        private readonly ISoundService _soundService;
        private readonly IAssetProvider _assetProvider;
        private readonly MusicTheme _musicTheme;

        public MusicPlayer(ISoundService soundService, IAssetProvider assetProvider, MusicTheme musicTheme)
        {
            _soundService = soundService;
            _assetProvider = assetProvider;
            _musicTheme = musicTheme;
        }

        public async UniTask PlayAsync(MusicType type, CancellationToken cancellationToken = default)
        {
            if (!_musicTheme.TryGet(type, out var entry))
                return;

            var clip = await _assetProvider.LoadAsync<AudioClip>(entry.Clip, cancellationToken);

            _soundService.PlayMusic(clip, entry.Loop);
        }

        public void Stop()
        {
            _soundService.StopMusic();
        }
    }
}
