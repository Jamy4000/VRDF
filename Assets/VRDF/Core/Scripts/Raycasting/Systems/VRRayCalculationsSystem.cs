using Unity.Entities;
using UnityEngine;
using VRDF.Core.SetupVR;

namespace VRDF.Core.Raycast
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
            if (VRDF_Components.SetupVRIsReady)
                AssignRay();
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        private void AssignRay()
        {
            Entities.ForEach((ref VRRaycastOrigin raycastOrigin, ref VRRaycastParameters parameters, ref VRRaycastOutputs raycastOutputs) =>
            {
                Transform originTransform = VRDF_Components.VRCamera.transform;

                // Depending on the RayOrigin, we provide references to different ray and raycastHit variables
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        originTransform = VRDF_Components.LeftController.transform;
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        originTransform = VRDF_Components.RightController.transform;
                        break;
                    case ERayOrigin.CAMERA:
                        // No need to set it, already done before
                        // originTransform = VRDF_Components.VRCamera.transform;
                        break;
                    default:
                        Debug.LogError("<b>[VRDF] :</b> An error has occured in the RayCalculationsSystems. " +
                            "Please check that the RayOrigin for your VRRaycatAuthoring Components are set correctly. Using Camera as Origin.");
                        break;
                }

                raycastOutputs.RayVar.origin = originTransform.position + ControllersRaycastOffset.RaycastPositionOffset[VRDF_Components.DeviceLoaded] + parameters.StartPointOffset;
                raycastOutputs.RayVar.direction = originTransform.TransformDirection(Vector3.forward + ControllersRaycastOffset.RaycastDirectionOffset[VRDF_Components.DeviceLoaded] + parameters.EndPointOffset);
            });
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRDF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
    }
}