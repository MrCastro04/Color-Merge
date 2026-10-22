using Assets.Game.Modules.Content.Item.Scripts;
using Assets.Game.Modules.Content.Level.Scripts;
using Assets.Game.Modules.Content.SFXOneShot.Scripts;
using Assets.Game.Modules.Content.Slot.Scripts;
using Assets.Game.Modules.Core.AssetManagement.Scripts;
using Assets.Game.Modules.Core.Infrastructure.Levels.Scripts;
using Assets.Game.Modules.Core.Infrastructure.MidLoader.Scripts;
using Assets.Game.Modules.Core.Save.Scripts;
using Assets.Game.Modules.Core.SceneManagement.Scripts;
using Assets.Game.Modules.Core.Sound.Scripts;
using Assets.Game.Modules.Core.UI.Scripts;
using Assets.Game.Modules.Core.Vibration.Scripts;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Assets.Game.Modules.Core.Infrastructure.Bootstrap.Scripts
{
    // Лежит в Boot.unity и живёт всю игру: общие сервисы (Singleton).
    // Сцены меню и уровней получают их через LevelLoader (LifetimeScope.EnqueueParent), как в Combo-Dungeon.
    public class ApplicationScope : LifetimeScope
    {
        [SerializeField] private ItemAssets _itemAssets;
        [SerializeField] private SlotAssets _slotAssets;

        [Header("Sound")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private MusicTheme _musicTheme;
        [SerializeField] private SFXOneShotConfig _sfxOneShotConfig;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterInstance(_itemAssets);
            builder.RegisterInstance(_slotAssets);
 
            builder.Register<AddressablesAssetProvider>(Lifetime.Singleton).As<IAssetProvider>().AsSelf();
            builder.Register<PlayerPrefsSaveService>(Lifetime.Singleton).As<ISaveService>();
            builder.Register<SceneLoader>(Lifetime.Singleton).As<ISceneLoader>();

            RegisterSound(builder);

            builder.Register<BestTimeRepository>(Lifetime.Singleton);
            builder.Register<LevelProgressRepository>(Lifetime.Singleton);
            RegisterLoading(builder);

            builder.RegisterEntryPoint<BootController>();
        }

        // Переходы между сценами: шторка (дочерний объект ApplicationScope) + цель загрузки + LevelLoader.
        private void RegisterLoading(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<ScreenFader>();
            builder.Register<LoadingTarget>(Lifetime.Singleton);
            builder.Register<LevelLoader>(Lifetime.Singleton).WithParameter<LifetimeScope>(this);
        }

        private void RegisterSound(IContainerBuilder builder)
        {
            builder.RegisterInstance(_musicTheme);
            builder.RegisterInstance(_sfxOneShotConfig);

            builder.Register<SoundService>(Lifetime.Singleton).As<ISoundService>().WithParameter(_musicSource);
            builder.Register<VibrationService>(Lifetime.Singleton).As<IVibrationService>();
            builder.Register<MusicPlayer>(Lifetime.Singleton);
            builder.Register<SFXOneShotPlayer>(Lifetime.Singleton);
        }
    }
}
