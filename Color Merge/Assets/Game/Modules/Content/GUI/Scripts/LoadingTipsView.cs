using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Game.Modules.Content.GUI.Scripts
{
    public class LoadingTipsView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private float _phraseDuration = 0.9f;
        [SerializeField] private List<string> _phrases = new()
        {
            "Loading materials...",
            "Loading levels...",
            "Mixing the colors...",
            "Polishing the gems...",
            "Sorting the tiles...",
            "Warming up the generator...",
            "Tuning the music...",
            "Almost ready...",
        };

        public void SetVisible(bool isVisible)
        {
            _text.gameObject.SetActive(isVisible);
        }

        public async UniTask PlayAsync(CancellationToken cancellationToken)
        {
            var order = new List<string>(_phrases);

            while (!cancellationToken.IsCancellationRequested)
            {
                Shuffle(order);

                foreach (var phrase in order)
                {
                    _text.text = phrase;
                    _text.alpha = 0f;
                    _text.DOFade(1f, 0.2f).SetLink(gameObject);

                    bool isCanceled = await UniTask
                        .Delay((int)(_phraseDuration * 1000), cancellationToken: cancellationToken)
                        .SuppressCancellationThrow();

                    if (isCanceled)
                        return;
                }
            }
        }

        private static void Shuffle(List<string> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                (list[i], list[j]) = (list[j], list[i]);
            }
        }
    }
}
