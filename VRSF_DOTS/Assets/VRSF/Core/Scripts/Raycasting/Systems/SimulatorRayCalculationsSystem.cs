using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Ray for the simulator
    /// CANNOT BE JOBIFIED; as Transform and Camera Component cannot be given in a job and we need them for either Transform.TransformDirection or Camera.ScreenPointToRay
    /// </summary>
    public class SimulatorRayCalculationsSystem : ComponentSystem
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
            Entities.ForEach((ref VRRaycastOrigin raycastOrigin, ref VRRaycastParameters parameters, ref VRRaycastOutputs raycastOutputs) =>
            {
                // We set the ray based on the raycastOrigin
                raycastOutputs.RayVar = raycastOrigin.RayOrigin == ERayOrigin.CAMERA ? new Ray(_mainCamera.transform.position, _mainCamera.transform.forward) : _mainCamera.ScreenPointToRay(Input.mousePosition);
            });
        }

        private void SetupCameraRef(OnSetupVRReady info)
        {
            if (VRSF_Components.DeviceLoaded == EDevice.SIMULATOR)
            {
                this.Enabled = true;
                _mainCamera = VRSF_Components.VRCamera.GetComponent<Camera>();
            }
            else
            {
                this.Enabled = false;
            }
        }
    }
}