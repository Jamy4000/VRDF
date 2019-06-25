using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserWidthSetter : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private ERayOrigin _rayOrigin;

        private float _newWidth = -1.0f;

        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            _rayOrigin = GetComponent<VRRaycastAuthoring>().RayOrigin;
            OnLaserWidthChanged.Listeners += UpdateLineRender;
        }

        void OnDestroy()
        {
            OnLaserWidthChanged.Listeners -= UpdateLineRender;
        }

        private void UpdateLineRender(OnLaserWidthChanged info)
        {
            if (info.LaserOrigin == _rayOrigin)
            {
                _lineRenderer.startWidth = info.NewWidth;
                _lineRenderer.endWidth = info.NewWidth;
            }
        }
    }
}