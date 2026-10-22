using System;
using System.Collections.Generic;
using Assets.Game.Modules.Core.UI.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    public class LevelSelectView : UIScreen
    {
        [SerializeField] private List<LevelButtonView> _levelButtons = new();
        [SerializeField] private Button _closeButton;

        private bool _isListening;

        public event Action<int> OnLevelSelected;
        public event Action<int> OnLockedLevelClicked;
        public event Action OnCloseClicked;

        public int LevelsCount => _levelButtons.Count;

        public void Setup(IReadOnlyList<LevelButtonState> states)
        {
            for (int i = 0; i < _levelButtons.Count; i++)
            {
                var state = i < states.Count ? states[i] : new LevelButtonState(true, false, 0f);
                _levelButtons[i].Setup(i, state);
            }

            if (_isListening)
                return;

            foreach (var levelButton in _levelButtons)
                levelButton.OnClicked += HandleOnLevelButtonClicked;

            _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
            _isListening = true;
        }

        public void SetInteractable(bool isInteractable)
        {
            if (_cg == null)
                _cg = GetComponent<CanvasGroup>();

            _cg.interactable = isInteractable;
        }

        private void OnDestroy()
        {
            foreach (var levelButton in _levelButtons)
            {
                if (levelButton != null)
                    levelButton.OnClicked -= HandleOnLevelButtonClicked;
            }
        }

        private void HandleOnLevelButtonClicked(LevelButtonView levelButton)
        {
            if (levelButton.IsLocked)
            {
                levelButton.PlayLockedShake();
                OnLockedLevelClicked?.Invoke(levelButton.Index);
                return;
            }

            OnLevelSelected?.Invoke(levelButton.Index);
        }
    }
}
