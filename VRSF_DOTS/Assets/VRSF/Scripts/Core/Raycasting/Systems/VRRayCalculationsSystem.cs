using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Ray of the two controllers and the Camera/Gaze
    /// CANNOT BE JOBIFIED; as Transform and Camera Component cannot be given in a job and we need them for either Transform.TransformDirection or Camera.ScreenPointToRay
    /// </summary>
    public class VRRayCalculationsSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            AssignRay();
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        private void AssignRay()
        {
            Entities.ForEach((ref VRRaycastOrigin raycastOrigin, ref VRRaycastParameters parameters) =>
            {
                // Depending on the RayOring, we provide references to different ray and raycastHit variables
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        Transform originTransform = VRSF_Components.LeftController.transform;
                        LeftControllerRaycastData.RayVar = new Ray(originTransform.position, originTransform.TransformDirection(Vector3.forward));
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        originTransform = VRSF_Components.RightController.transform;
                        RightControllerRaycastData.RayVar = new Ray(originTransform.position, originTransform.TransformDirection(Vector3.forward));
                        break;
                    case ERayOrigin.CAMERA:
                        originTransform = VRSF_Components.VRCamera.transform;
                        CameraRaycastData.RayVar = new Ray(originTransform.position, originTransform.TransformDirection(Vector3.forward));
                        break;

                    default:
                        Debug.LogError("[b]VRSF :[\b] An error has occured in the RayCalculationsSystems. " +
                            "Please check that the RayOrigin for your VRRaycatAuthoring Components are set correctly.");
                        break;
                }
            });
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
    }
}