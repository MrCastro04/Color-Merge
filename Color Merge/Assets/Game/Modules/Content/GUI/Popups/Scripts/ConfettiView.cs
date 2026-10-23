using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.GUI.Popups.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class ConfettiView : MonoBehaviour
    {
        [SerializeField] private Texture _texture;
        [SerializeField] private int _count = 70;
        [SerializeField] private Vector2 _pieceSize = new(56f, 56f);
        [SerializeField] private float _duration = 2.2f;

        private RectTransform _rt;

        private void Awake()
        {
            _rt = (RectTransform)transform;
        }

        public async UniTask PlayAsync(CancellationToken cancellationToken = default)
        {
            if (_rt == null)
                _rt = (RectTransform)transform;

            var area = _rt.rect.size;

            for (int i = 0; i < _count; i++)
                SpawnPiece(area);

            await UniTask.Delay((int)(_duration * 1000), cancellationToken: cancellationToken)
                .SuppressCancellationThrow();
        }

        private void SpawnPiece(Vector2 area)
        {
            var go = new GameObject("Confetti", typeof(RectTransform), typeof(RawImage));
            go.layer = gameObject.layer;

            var piece = (RectTransform)go.transform;
            piece.SetParent(_rt, false);
            piece.sizeDelta = _pieceSize * Random.Range(0.7f, 1.2f);
            piece.anchoredPosition = new Vector2(Random.Range(-60f, 60f), Random.Range(-40f, 40f));
            piece.localRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));

            var image = go.GetComponent<RawImage>();
            image.texture = _texture;
            image.raycastTarget = false;
            image.uvRect = new Rect(Random.Range(0, 2) * 0.5f, Random.Range(0, 2) * 0.5f, 0.5f, 0.5f);

            var burst = new Vector2(Random.Range(-area.x * 0.5f, area.x * 0.5f), Random.Range(area.y * 0.1f, area.y * 0.45f));
            var fall = new Vector2(burst.x + Random.Range(-120f, 120f), -area.y * 0.6f);
            float burstTime = _duration * Random.Range(0.25f, 0.35f);
            float fallTime = _duration - burstTime;

            DOTween.Sequence()
                .Append(piece.DOAnchorPos(burst, burstTime).SetEase(Ease.OutCubic))
                .Append(piece.DOAnchorPos(fall, fallTime).SetEase(Ease.InSine))
                .Join(piece.DORotate(new Vector3(0, 0, Random.Range(-720f, 720f)), fallTime, RotateMode.FastBeyond360))
                .Insert(_duration * 0.75f, image.DOFade(0f, _duration * 0.25f))
                .SetLink(go)
                .OnComplete(() => Destroy(go));
        }
    }
}
