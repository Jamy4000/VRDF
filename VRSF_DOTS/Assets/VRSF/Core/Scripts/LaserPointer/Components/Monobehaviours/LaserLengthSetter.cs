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

        void Awake()
        {
            _lineRenderer = GetComponent<LineRenderer>();
            // VRRaycastAuthoring is necessaraly on this GameObject; as the LaserPointerStateAuthoring needs it.
            _rayOrigin = GetComponent<VRRaycastAuthoring>().RayOrigin;
            OnLaserLengthChanged.Listeners += UpdateLineRender;
            OnSetupVRReady.RegisterSetupVRResponse(InitPositionOffset);
        }

        void OnDestroy()
        {
            OnLaserLengthChanged.Listeners -= UpdateLineRender;

            if (OnSetupVRReady.IsMethodAlreadyRegistered(InitPositionOffset))
                OnSetupVRReady.Listeners -= InitPositionOffset;
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

        private void InitPositionOffset(OnSetupVRReady info)
        {
            transform.localPosition += ControllersRaycastOffset.RaycastPositionOffset[VRSF_Components.DeviceLoaded];
        }
    }
}