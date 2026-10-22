using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Content.Tutorial.Tutor_Scenario.Scripts
{
    public class MoveableTutorArrow : BaseTutorArrow
    {
        [SerializeField] private RectTransform _rtArrow;
        [SerializeField] private RectTransform _rtMoveTo;

        public float DurationAnimation = 1.5f;

        private Tween _tweenMoveTo;

        private void OnDisable()
        {
            Stop();
        }

        public void Play(float duration)
        {
            Stop();

            _tweenMoveTo = _rtArrow
                .DOMove(_rtMoveTo.position, duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        public void Play(Vector3 from, Vector3 to)
        {
            _rtArrow.position = from;
            _rtMoveTo.position = to;

            Play(DurationAnimation);
        }

        public void Stop()
        {
            _tweenMoveTo?.Kill();
            _tweenMoveTo = null;
        }
    }
}
