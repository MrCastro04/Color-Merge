using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.UI.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class ScreenShakeView : MonoBehaviour
    {
        private RectTransform _rt;
        private Tween _shake;
        private Vector2 _restPosition;

        private void Awake()
        {
            _rt = GetComponent<RectTransform>();
        }

        public void Shake(float duration, float strength, int vibrato)
        {
            if (duration <= 0f || strength <= 0f)
                return;

            if (_shake == null)
                _restPosition = _rt.anchoredPosition;

            _shake?.Kill();
            _rt.anchoredPosition = _restPosition;

            _shake = _rt
                .DOShakeAnchorPos(duration, strength, vibrato, 90f, false, true)
                .OnComplete(ResetPosition)
                .OnKill(() => _shake = null)
                .SetLink(gameObject);
        }

        private void ResetPosition()
        {
            _rt.anchoredPosition = _restPosition;
        }
    }
}
