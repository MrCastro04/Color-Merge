using System.Threading;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.UI.Scripts
{
    // Базовый экран: показ/скрытие через фейд CanvasGroup.
    [RequireComponent(typeof(CanvasGroup))]
    public class UIScreen : MonoBehaviour
    {
        [SerializeField] protected float _fadeDuration = 0.25f;

        protected CanvasGroup _cg;

        public bool IsVisible => gameObject.activeSelf;

        protected virtual void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
        }

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            gameObject.SetActive(true);

            _cg.interactable = false;
            _cg.blocksRaycasts = true;
            _cg.alpha = 0f;

            await _cg
                .DOFade(1f, _fadeDuration)
                .SetLink(gameObject)
                .AwaitCompletion(cancellationToken);

            _cg.interactable = true;
        }

        public virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            _cg.interactable = false;

            await _cg
                .DOFade(0f, _fadeDuration)
                .SetLink(gameObject)
                .AwaitCompletion(cancellationToken);

            _cg.blocksRaycasts = false;
            gameObject.SetActive(false);
        }

        public void HideImmediate()
        {
            if (_cg == null)
                _cg = GetComponent<CanvasGroup>();

            _cg.alpha = 0f;
            _cg.interactable = false;
            _cg.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }
}
