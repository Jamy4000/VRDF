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
                // We set the ray based on the raycastOrigin if not simulator, and the ScreenPointToRay method if we use the Simulator
                raycastOutputs.RayVar = _mainCamera.ScreenPointToRay(Input.mousePosition);
                raycastOrigin.RayOriginPosition = VRSF_Components.VRCamera.transform.position;
            });
        }

        private void SetupCameraRef(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
            _mainCamera = VRSF_Components.VRCamera.GetComponent<Camera>();
        }
    }
}