using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Ray of the two controllers and the Camera/Gaze, and reference them in RaycastHit and Ray static classes
    /// </summary>
    public class RayCalculationsSystems : ComponentSystem
    {
        private Camera _mainCamera;

        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += SetupCameraRef;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            if (_mainCamera != null)
                AssignRay();
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= SetupCameraRef;
            base.OnDestroy();
        }

        private void AssignRay()
        {
            Entities.ForEach((ref VRRaycastOrigin raycastOrigin, ref VRRaycastParameters parameters) =>
            {
                // We get the origin for this raycast
                Transform originTransform = GetOriginTransform(raycastOrigin.RayOrigin);

                // We set the ray based on the raycastOrigin if not simulator, and the ScreenPointToRay method if we use the Simulator
                Ray ray = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR ? _mainCamera.ScreenPointToRay(Input.mousePosition) : new Ray(originTransform.position, originTransform.TransformDirection(Vector3.forward));
                
                // Depending on the RayOring, we provide references to different ray and raycastHit variables
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        LeftControllerRaycastData.RayVar = ray;
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        RightControllerRaycastData.RayVar = ray;
                        break;
                    case ERayOrigin.CAMERA:
                        CameraRaycastData.RayVar = ray;
                        break;

                    default:
                        Debug.LogError("[b]VRSF :[\b] An error has occured in the RayCalculationsSystems. " +
                            "Please check that the RayOrigin for your VRRaycatAuthoring Components are set correctly.");
                        break;
                }

                Transform GetOriginTransform(ERayOrigin rayOrigin)
                {
                    switch (rayOrigin)
                    {
                        case ERayOrigin.LEFT_HAND:
                            return VRSF_Components.LeftController.transform;

                        case ERayOrigin.RIGHT_HAND:
                            return VRSF_Components.RightController.transform;

                        case ERayOrigin.CAMERA:
                            return VRSF_Components.VRCamera.transform;

                        default:
                            Debug.LogError("[b]VRSF :[\b] An error has occured in the RayCalculationsSystems. " +
                                "Please check that the RayOrigin for your VRRaycatAuthoring Components are set correctly. Returning Camera as origin.");
                            return VRSF_Components.VRCamera.transform;
                    }
                }
            });
        }

        private void SetupCameraRef(OnSetupVRReady info)
        {
            _mainCamera = VRSF_Components.VRCamera.GetComponent<Camera>();
        }
    }
}