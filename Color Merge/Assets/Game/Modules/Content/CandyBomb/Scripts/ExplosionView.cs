using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.CandyBomb.Scripts
{
    [RequireComponent(typeof(RectTransform))]
    public class ExplosionView : MonoBehaviour
    {
        [SerializeField] private Texture _texture;
        [SerializeField] private Sprite _flashSprite;
        [SerializeField] private int _count = 40;
        [SerializeField] private Vector2 _pieceSize = new(90f, 90f);
        [SerializeField] private float _duration = 0.6f;

        public void Play(Vector3 worldPosition, float radiusWorld)
        {
            PlayFlash(worldPosition, radiusWorld);

            for (int i = 0; i < _count; i++)
                SpawnPiece(worldPosition, radiusWorld);
        }

        private void PlayFlash(Vector3 worldPosition, float radiusWorld)
        {
            if (_flashSprite == null)
                return;

            var go = new GameObject("Flash", typeof(RectTransform), typeof(Image));
            var rt = (RectTransform)go.transform;
            rt.SetParent(transform, false);
            rt.sizeDelta = _pieceSize;
            rt.position = worldPosition;

            var flash = go.GetComponent<Image>();
            flash.sprite = _flashSprite;
            flash.preserveAspect = true;
            flash.raycastTarget = false;

            float scale = radiusWorld * 2f / Mathf.Max(1f, _pieceSize.x * rt.lossyScale.x);

            rt.localScale = Vector3.zero;
            DOTween.Sequence()
                .Append(rt.DOScale(scale, _duration * 0.4f).SetEase(Ease.OutQuad))
                .Join(rt.DORotate(new Vector3(0f, 0f, 180f), _duration * 0.8f))
                .Join(flash.DOFade(0f, _duration * 0.5f).SetDelay(_duration * 0.3f))
                .OnComplete(() => Destroy(flash.gameObject))
                .SetLink(flash.gameObject);
        }

        private void SpawnPiece(Vector3 worldPosition, float radiusWorld)
        {
            var uv = new Rect(Random.Range(0, 2) * 0.5f, Random.Range(0, 2) * 0.5f, 0.5f, 0.5f);
            var piece = CreatePiece("Piece", worldPosition, uv);
            var rt = (RectTransform)piece.transform;

            Vector3 direction = Random.insideUnitCircle.normalized;
            Vector3 target = worldPosition + direction * radiusWorld * Random.Range(0.6f, 1.1f);
            float duration = _duration * Random.Range(0.8f, 1.2f);

            rt.localScale = Vector3.one * Random.Range(0.5f, 1f);
            rt.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

            DOTween.Sequence()
                .Append(rt.DOMove(target, duration).SetEase(Ease.OutCubic))
                .Join(rt.DORotate(new Vector3(0f, 0f, Random.Range(-360f, 360f)), duration, RotateMode.FastBeyond360))
                .Join(piece.DOFade(0f, duration * 0.35f).SetDelay(duration * 0.65f))
                .OnComplete(() => Destroy(piece.gameObject))
                .SetLink(piece.gameObject);
        }

        private RawImage CreatePiece(string pieceName, Vector3 worldPosition, Rect uv)
        {
            var go = new GameObject(pieceName, typeof(RectTransform), typeof(RawImage));
            var rt = (RectTransform)go.transform;
            rt.SetParent(transform, false);
            rt.sizeDelta = _pieceSize;
            rt.position = worldPosition;

            var image = go.GetComponent<RawImage>();
            image.texture = _texture;
            image.uvRect = uv;
            image.raycastTarget = false;
            return image;
        }
    }
}
