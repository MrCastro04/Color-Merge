using System.Threading;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Assets.Game.Modules.Core.UI.Scripts
{
    [RequireComponent(typeof(CanvasGroup))]
    public class ScreenFader : MonoBehaviour
    {
        private CanvasGroup _cg;

        public bool IsVisible => _cg.alpha > 0f;

        private void Awake()
        {
            _cg = GetComponent<CanvasGroup>();
            _cg.alpha = 1f;
            _cg.blocksRaycasts = true;
        }

        public UniTask FadeInAsync(float duration = 0.3f, CancellationToken cancellationToken = default)
        {
            _cg.DOKill();
            _cg.blocksRaycasts = true;

            return _cg
                .DOFade(1f, duration)
                .SetUpdate(true)
                .SetLink(gameObject)
                .AwaitCompletion(cancellationToken);
        }

        public async UniTask FadeOutAsync(float duration = 0.35f, CancellationToken cancellationToken = default)
        {
            _cg.DOKill();

            await _cg
                .DOFade(0f, duration)
                .SetUpdate(true)
                .SetLink(gameObject)
                .AwaitCompletion(cancellationToken);

            _cg.blocksRaycasts = false;
        }
    }
}
