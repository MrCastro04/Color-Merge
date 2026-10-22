using Assets.Game.Modules.Core.Save.Scripts;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Game.Modules.Core.Sound.Scripts
{
    public class SoundService : ISoundService
    {
        private const string MUSIC_VOLUME_KEY = "sound_music_volume";
        private const string SFX_VOLUME_KEY = "sound_sfx_volume";

        private const float DEFAULT_MUSIC_VOLUME = 0.06f;
        private const float DEFAULT_SFX_VOLUME = 0.06f;

        private const float MIN_PITCH = 0.92f;
        private const float MAX_PITCH = 1.08f;

        private readonly AudioSource _musicSource;
        private readonly ISaveService _saveService;

        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
        public bool IsMusicPlaying => _musicSource.isPlaying;
        public AudioClip CurrentMusic => _musicSource.clip;

        public SoundService(AudioSource musicSource, ISaveService saveService)
        {
            _musicSource = musicSource;
            _saveService = saveService;

            MusicVolume = _saveService.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_MUSIC_VOLUME);
            SfxVolume = _saveService.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);

            _musicSource.playOnAwake = false;
            _musicSource.spatialBlend = 0f;
            _musicSource.volume = MusicVolume;
        }

        public void PlayMusic(AudioClip clip, bool loop = true)
        {
            if (clip == null)
                return;

            _musicSource.loop = loop;

            if (_musicSource.clip == clip && _musicSource.isPlaying)
                return;

            _musicSource.clip = clip;
            _musicSource.volume = MusicVolume;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public void PlayOneShot(AudioClip clip, float volumeScale = 1f, bool randomizePitch = true)
        {
            if (clip == null || SfxVolume <= 0f)
                return;

            var go = new GameObject($"SFX {clip.name}");
            go.transform.SetParent(_musicSource.transform, false);

            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.clip = clip;
            source.volume = SfxVolume * volumeScale;

            if (randomizePitch)
                source.pitch = Random.Range(MIN_PITCH, MAX_PITCH);
            else
                source.pitch = 1f;
            source.Play();

            Object.Destroy(go, clip.length / source.pitch + 0.1f);
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = Mathf.Clamp01(volume);
            _musicSource.volume = MusicVolume;

            _saveService.SetFloat(MUSIC_VOLUME_KEY, MusicVolume);
            _saveService.Save();
        }

        public void SetSfxVolume(float volume)
        {
            SfxVolume = Mathf.Clamp01(volume);

            _saveService.SetFloat(SFX_VOLUME_KEY, SfxVolume);
            _saveService.Save();
        }
    }
}
