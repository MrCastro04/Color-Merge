using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.CandyBomb.Scripts
{
    // Затемнение на время взрыва конфетной бомбы (как на туторе). Свой Canvas на слое Tutor —
    // бомба и взрыв лежат на Drag Canvas (слой Drag) и видны поверх, всё остальное гаснет.
    // Пока затемнение на экране, оно ловит нажатия (свой GraphicRaycaster).
    public class BombDimView : MonoBehaviour
    {
        [SerializeField] private Image _image;

        private void Awake()
        {
            SetAlpha(0f);
            gameObject.SetActive(false);
        }

        public void Show(float alpha, float duration)
        {
            gameObject.SetActive(true);

            _image.DOKill();
            _image
                .DOFade(alpha, duration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject);
        }

        public void Hide(float duration)
        {
            _image.DOKill();
            _image
                .DOFade(0f, duration)
                .SetEase(Ease.InQuad)
                .OnComplete(() => gameObject.SetActive(false))
                .SetLink(gameObject);
        }

        private void SetAlpha(float alpha)
        {
            var color = _image.color;
            color.a = alpha;
            _image.color = color;
        }
    }
}
