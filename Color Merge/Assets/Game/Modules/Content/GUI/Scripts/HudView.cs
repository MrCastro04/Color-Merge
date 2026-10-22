using Assets.Game.Modules.Core.Timer.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts;
using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Assets.Game.Modules.Content.GUI.Scripts
{
    public class HudView : MonoBehaviour, IChangeSortGroupInCanvas
    {
        private const float WARNING_TIME = 10f;

        [SerializeField] private TMP_Text _timeText;
        [SerializeField] private TMP_Text _bestTimeText;
        [SerializeField] private TMP_Text _levelText;
        [SerializeField] private TMP_Text _mergesText;
        [SerializeField] private LevelGoalsView _goals;
        [SerializeField] private Color _warningColor = new(1f, 0.35f, 0.35f, 1f);
        [SerializeField] private Canvas _sortingCanvas;

        private Color _timeColor;
        private bool _isColorCached;

        public string DesiredGroup => SORTING_LAYERS.TUTOR;
        public string StartGroup { get; set; } = SORTING_LAYERS.DEFAULT;
        public Canvas SortingCanvas => _sortingCanvas;

        public Bounds TimeTextWorldBounds
        {
            get
            {
                _timeText.ForceMeshUpdate();
                var bounds = _timeText.textBounds;
                var t = _timeText.transform;

                return new Bounds(t.TransformPoint(bounds.center), Vector3.Scale(bounds.size, t.lossyScale));
            }
        }
        private bool _isWarning;

        public void SetTimerVisible(bool isVisible)
        {
            _timeText.gameObject.SetActive(isVisible);

            if (!isVisible)
                _bestTimeText.gameObject.SetActive(false);
        }

        public void SetRemainingTime(float seconds)
        {
            _timeText.text = TimeFormatter.Format(seconds);
            SetWarning(seconds <= WARNING_TIME);
        }

        private void SetWarning(bool isWarning)
        {
            if (!_isColorCached)
            {
                _timeColor = _timeText.color;
                _isColorCached = true;
            }

            if (isWarning == _isWarning)
                return;

            _isWarning = isWarning;

            if (isWarning)
                _timeText.color = _warningColor;
            else
                _timeText.color = _timeColor;

            if (isWarning)
                _timeText.transform.DOPunchScale(Vector3.one * 0.15f, 0.4f, 4, 0.5f).SetLink(gameObject);
        }

        public void SetBestTime(bool hasBestTime, float seconds)
        {
            _bestTimeText.gameObject.SetActive(hasBestTime);

            if (hasBestTime)
                _bestTimeText.text = $"BEST {TimeFormatter.Format(seconds)}";
        }

        public void SetMerges(int done, int target)
        {
            if (_mergesText == null)
                return;

            _mergesText.gameObject.SetActive(target > 0);
            _mergesText.text = $"MERGES {Mathf.Min(done, target)}/{target}";
        }

        public RectTransform MergesTextRT => (RectTransform)_mergesText.transform;

        public LevelGoalsView Goals => _goals;

        public void SetLevelName(string levelName)
        {
            if (_levelText != null)
                _levelText.text = levelName;
        }
    }
}
