using UnityEngine;

namespace Assets.Game.Modules.Content.Item.Scripts
{
    [CreateAssetMenu(menuName = "Game/Item/Candy Bomb Config", fileName = "Candy_Bomb")]
    public class CandyBombConfig : ItemConfig
    {
        public const int MIN_RADIUS = 1;

        [Tooltip("Радиус взрыва в клетках вокруг бомбы")]
        [SerializeField, Min(MIN_RADIUS)] private int _radius = 1;

        [Header("Затемнение")]
        [SerializeField, Range(0f, 1f)] private float _dimAlpha = 0.7f;
        [Tooltip("Затемнение появляется, когда бомбу отпустили на клетку")]
        [SerializeField, Min(0f)] private float _dimFadeInDuration = 0.2f;
        [Tooltip("Затемнение уходит сразу после взрыва")]
        [SerializeField, Min(0f)] private float _dimFadeOutDuration = 0.15f;

        [Header("Тряска экрана")]
        [SerializeField, Min(0f)] private float _shakeDuration = 0.35f;
        [Tooltip("Амплитуда в единицах канваса (1080×1920)")]
        [SerializeField, Min(0f)] private float _shakeStrength = 25f;
        [SerializeField, Min(1)] private int _shakeVibrato = 20;

        public int Radius => Mathf.Max(MIN_RADIUS, _radius);
        public float DimAlpha => _dimAlpha;
        public float DimFadeInDuration => _dimFadeInDuration;
        public float DimFadeOutDuration => _dimFadeOutDuration;
        public float ShakeDuration => _shakeDuration;
        public float ShakeStrength => _shakeStrength;
        public int ShakeVibrato => _shakeVibrato;
    }
}
