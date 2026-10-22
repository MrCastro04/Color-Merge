using System.Collections.Generic;
using System.Threading;
using Assets.Game.Modules.Content.Tutorial.Arrows;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using Febucci.UI;
using UnityEngine;

namespace Assets.Game.Modules.Content.Tutorial
{
    public class Tutor : MonoBehaviour, IWaiter
    {
        [SerializeField] protected GameObject _containerContent;
        [SerializeField] protected TypewriterByCharacter _typewriter;
        [SerializeField, TextArea(10, 200)] protected string _text;
        [SerializeField] protected ArrowContainer[] _arrows;

        [HideInInspector] public bool IsActive;
        public bool IsTyping => _typewriter.isShowingText;

        public IReadOnlyList<ArrowContainer> Arrows => _arrows;

        private void OnEnable()
        {
            _containerContent.SetActive(false);
            UpdateActiveState();
        }

        public async UniTask ShowText(CancellationToken ct = default)
        {
            if (this is IWindowObj windowObj)
            {
                await windowObj.OpenAsWindow(ct);
            }
            else
            {
                _containerContent.SetActive(true);
            }

            UpdateActiveState();

            _typewriter.ShowText(_text);

            await this.WaitUntil(() => _typewriter.isShowingText == false, ct);
        }

        public UniTask Hide()
        {
            _containerContent.SetActive(false);
            UpdateActiveState();

            return UniTask.CompletedTask;
        }

        public void SetVisible(bool value)
        {
            _containerContent.SetActive(value);
            UpdateActiveState();
        }

        private void UpdateActiveState()
        {
            IsActive = _containerContent != null && _containerContent.activeSelf;
        }

        public void SkipTyping()
        {
            _typewriter.SkipTypewriter();
        }
    }
}
