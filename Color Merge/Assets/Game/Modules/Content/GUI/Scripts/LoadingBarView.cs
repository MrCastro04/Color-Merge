using System.Threading;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Game.Modules.Content.GUI.Scripts
{
    public class LoadingBarView : MonoBehaviour
    {
        [SerializeField] private RectTransform _fill;
        [SerializeField] private TMP_Text _percentText;

        private float _value;
        private Tween _tween;

        public float Value => _value;

        public void Init()
        {
            SetValue(0f);
        }

        public UniTask AnimateToAsync(float target, float speed = 1.2f, CancellationToken cancellationToken = default)
        {
            target = Mathf.Clamp01(target);
            float duration = Mathf.Abs(target - _value) / speed;

            _tween?.Kill();
            _tween = DOTween
                .To(() => _value, SetValue, target, duration)
                .SetEase(Ease.OutSine)
                .SetLink(gameObject);

            return _tween.AwaitCompletion(cancellationToken);
        }

        private void SetValue(float value)
        {
            _value = value;

            var anchorMax = _fill.anchorMax;
            anchorMax.x = value;
            _fill.anchorMax = anchorMax;

            if (_percentText != null)
                _percentText.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }
}
