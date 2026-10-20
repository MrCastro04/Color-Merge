using UnityEngine;

namespace Assets.Game.Modules.Core.Sound.Scripts
{
    public interface ISoundService
    {
        float MusicVolume { get; }
        float SfxVolume { get; }
        bool IsMusicPlaying { get; }
        AudioClip CurrentMusic { get; }

        void PlayMusic(AudioClip clip, bool loop = true);
        void StopMusic();

        void PlayOneShot(AudioClip clip, float volumeScale = 1f, bool randomizePitch = true);

        void SetMusicVolume(float volume);
        void SetSfxVolume(float volume);
    }
}
