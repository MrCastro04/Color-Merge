using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Game.Modules.Content.Generator.Scripts
{
    public class GeneratorButtonView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private TMP_Text _chargesText;

        public event Action OnClicked;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleOnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleOnClick);
        }

        // charges < 0 — без ограничения, счётчик скрыт
        public void SetCharges(int charges)
        {
            if (_chargesText == null)
                return;

            _chargesText.gameObject.SetActive(charges >= 0);
            _chargesText.text = charges.ToString();
        }

        public void SetInteractable(bool isInteractable)
        {
            _button.interactable = isInteractable;
        }

        private void HandleOnClick()
        {
            transform.DOKill(true);
            transform.DOPunchScale(Vector3.one * -0.1f, 0.2f, 6, 0.5f).SetLink(gameObject);

            OnClicked?.Invoke();
        }
    }
}
