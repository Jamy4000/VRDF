using System;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.Core.LaserPointer
{
    [RequireComponent(typeof(LineRenderer))]
    public class LaserLengthSetter : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private ERayOrigin _rayOrigin;
        private Vector3 _raycastStartPosOffset;

        void Awake()
        {
            _raycastStartPosOffset = GetComponent<VRRaycastAuthoring>().StartPointOffset;
            _lineRenderer = GetComponent<LineRenderer>();
            // VRRaycastAuthoring is necessaraly on this GameObject; as the LaserPointerStateAuthoring needs it.
            _rayOrigin = GetComponent<VRRaycastAuthoring>().RayOrigin;
            OnLaserLengthChanged.Listeners += UpdateLineRender;
            OnSetupVRReady.RegisterSetupVRCallback(InitPos);
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
                    Vector3.zero,
                    info.NewEndPos
                });
            }
        }

        private void InitPos(OnSetupVRReady _)
        {
            OnSetupVRReady.UnregisterSetupVRCallback(InitPos);
            transform.localPosition = _raycastStartPosOffset + ControllersRaycastOffset.RaycastPositionOffset[VRSF_Components.DeviceLoaded];
        }
    }
}