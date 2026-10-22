using Assets.Game.Modules.Core.Utils.Scripts.Extensions;
using UnityEngine;

namespace Assets.Game.Modules.Content.Tutorial.Arrows
{
    public class ArrowView : MonoBehaviour, IMoveArrow
    {
        private RectTransform _rt;
        private ArrowDirectionType _arrowDirectionType;

        public RectTransform RT => _rt;
        public ArrowDirectionType ArrowDirectionType => _arrowDirectionType;
        public Vector3 EndPoint { get; set; }
        
        private Quaternion _rtRotation
        {
            get => _rt.rotation;
            set => _rt.rotation = value;
        }

        public void Init(ArrowDirectionType arrowDirectionType)
        {
            _rt = GetComponent<RectTransform>();
            _arrowDirectionType = arrowDirectionType;
            
            RotateAndSetEndPoint(arrowDirectionType);
        }
        
        public void RotateAndSetEndPoint(ArrowDirectionType arrowDirectionType)
        {
            switch (arrowDirectionType)
            {
                case ArrowDirectionType.RIGHT:
                    _rtRotation = Quaternion.Euler(0f, 0f, 0f);
                    EndPoint = Vector3.right;
                    break;
                case ArrowDirectionType.LEFT:
                    _rtRotation = Quaternion.Euler(0f, 0f, 180f);
                    EndPoint = Vector3.left;
                    break;
                case ArrowDirectionType.UP:
                    _rtRotation = Quaternion.Euler(0f, 0f, 90f);
                    EndPoint = Vector3.up;
                    break;
                case ArrowDirectionType.DOWN:
                    _rtRotation = Quaternion.Euler(0f, 0f, -90f);
                    EndPoint = Vector3.down;
                    break;
            }
        }
    }
}