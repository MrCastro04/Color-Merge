using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Content.Tutorial.Arrows
{
    [RequireComponent(typeof(RectTransform))]
    public class ArrowContainer : MonoBehaviour, IMovator
    {
        [SerializeField] RectTransform _rt;

        public ArrowDirectionType Direction = ArrowDirectionType.RIGHT;

        // Ход «туда-обратно» в единицах канваса (Screen Space - Overlay: переводится в пиксели экрана через масштаб канваса)
        public float MoveDistance = 30f;

        [Min(0.01f)] public float DurationAnimation = 0.5f;

        // От центра стрелки до её острия, в единицах канваса (картинка 100 × масштаб 1.5).
        public float TipOffset = 75f;


        private Vector3 _startPosition;
        private Tween _moveTween;

        public RectTransform RT => _rt;

        private void OnEnable()
        {
            if (_rt == null)
                _rt = GetComponent<RectTransform>();

            _startPosition = _rt.position;
            RotateAndAnimate();
        }

        private void OnDisable()
        {
            StopAnimation();

            if (_rt != null)
                _rt.position = _startPosition;
        }

        // Ставит стрелку перед целью — остриём к её краю со стороны Direction (DOWN — сверху, RIGHT — слева и т.д.).
        // Цели, которые зависят от раскладки (иконка цели в цепочке, генератор), задаются в рантайме.
        public void PointAt(RectTransform target, float gap = 10f)
        {
            if (_rt == null)
                _rt = GetComponent<RectTransform>();

            var corners = new Vector3[4];
            target.GetWorldCorners(corners);

            PointAt(new Bounds((corners[0] + corners[2]) * 0.5f, corners[2] - corners[0]), gap);
        }

        // То же по мировым границам (например, только цифры таймера, а не всё поле текста).
        public void PointAt(Bounds targetBounds, float gap = 10f)
        {
            if (_rt == null)
                _rt = GetComponent<RectTransform>();

            Vector3 direction = GetDirectionVector();
            float halfSize = Mathf.Abs(Vector3.Dot(targetBounds.extents, direction));
            float scale = _rt.lossyScale.x;

            _rt.position = targetBounds.center - direction * (halfSize + (TipOffset + MoveDistance + gap) * scale);
            _startPosition = _rt.position;

            if (isActiveAndEnabled)
                RotateAndAnimate();
            else
                RotateArrow();
        }

        private void RotateAndAnimate()
        {
            RotateArrow();
            StopAnimation();

            _rt.position = _startPosition;

            Vector3 offset = (Vector3)GetDirectionVector() * (MoveDistance * _rt.lossyScale.x);

            _moveTween = this.MoveYoyo(_rt.position + offset, DurationAnimation)
                .SetEase(Ease.InOutSine)
                .SetLink(gameObject);
        }

        private void StopAnimation()
        {
            _moveTween?.Kill();
            _moveTween = null;
        }

        private void RotateArrow()
        {
            if (_rt == null)
                return;

            float angle = Direction switch
            {
                ArrowDirectionType.UP => 90f,
                ArrowDirectionType.DOWN => -90f,
                ArrowDirectionType.LEFT => 180f,
                _ => 0f
            };

            _rt.localRotation = Quaternion.Euler(0f, 0f, angle);
        }

        private Vector2 GetDirectionVector()
        {
            return Direction switch
            {
                ArrowDirectionType.UP => Vector2.up,
                ArrowDirectionType.DOWN => Vector2.down,
                ArrowDirectionType.LEFT => Vector2.left,
                _ => Vector2.right
            };
        }
    }
}
