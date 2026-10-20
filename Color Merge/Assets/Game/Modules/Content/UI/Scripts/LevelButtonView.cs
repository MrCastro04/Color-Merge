using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.UI.Scripts
{
    // Кнопка уровня в окне выбора: номер или замок. Закрытая кнопка нажимается (для звука «нельзя») и покачивается.
    // Не зависит от Awake: окно выбора может быть выключено на сцене до первого показа.
    public class LevelButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _numberText;
        [SerializeField] private GameObject _lockIcon;
        [SerializeField] private Image _background;
        [SerializeField] private Color _unlockedColor = Color.white;
        [SerializeField] private Color _lockedColor = new(0.6f, 0.6f, 0.65f, 1f);

        private bool _isListening;

        public int Index { get; private set; }
        public bool IsLocked { get; private set; }

        public event Action<LevelButtonView> OnClicked;

        public void Setup(int index, bool isLocked)
        {
            Index = index;
            IsLocked = isLocked;

            _numberText.text = (index + 1).ToString();
            _numberText.gameObject.SetActive(!isLocked);
            _lockIcon.SetActive(isLocked);
            _background.color = isLocked ? _lockedColor : _unlockedColor;

            if (!_isListening)
            {
                _button.onClick.AddListener(() => OnClicked?.Invoke(this));
                _isListening = true;
            }
        }

        public void PlayLockedShake()
        {
            transform.DOKill(true);
            transform.DOShakeRotation(0.35f, new Vector3(0, 0, 12f), 12, 0f).SetLink(gameObject);
        }
    }
}
