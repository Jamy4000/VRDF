using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserLengthSetter : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private ERayOrigin _rayOrigin;

        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            // VRRaycastAuthoring is necessaraly on this GameObject; as the LaserPointerStateAuthoring needs it.
            _rayOrigin = GetComponent<VRRaycastAuthoring>().RayOrigin;
            OnLaserLengthChanged.Listeners += UpdateLineRender;
        }

        void OnDestroy()
        {
            OnLaserLengthChanged.Listeners -= UpdateLineRender;
        }

        private void UpdateLineRender(OnLaserLengthChanged info)
        {
            if (info.LaserOrigin == _rayOrigin)
            {
                _lineRenderer.SetPositions(new Vector3[]
                {
                    transform.position,
                    info.NewEndPos
                });
            }
        }
    }
}