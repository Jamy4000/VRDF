using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Simulator
{
    [UpdateBefore(typeof(SimulatorRotationSystem))]
    public class SimulatorCameraStateSystem : ComponentSystem
    {
        private bool _setupDone = true;

        protected override void OnCreateManager()
        {
            OnSetupVRReady.RegisterSetupVRResponse(CheckSystemState);
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            if (!_setupDone)
            {
                Entities.ForEach((ref SimulatorCameraState scs) =>
                {
                    ResetCameraStateTransform(ref scs);
                    _setupDone = true;
                });
            }

            if (Input.GetMouseButtonDown(1))
            {
                Entities.ForEach((ref SimulatorCameraState scs) =>
                {
                    ResetCameraStateTransform(ref scs);
                });
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(CheckSystemState))
                OnSetupVRReady.Listeners -= CheckSystemState;
        }

        public static void ResetCameraStateTransform(ref SimulatorCameraState scs)
        {
            scs.InterpolatingCameraState.SetFromTransform(VRSF_Components.CameraRig.transform);
            scs.TargetCameraState.SetFromTransform(VRSF_Components.CameraRig.transform);
        }
        
        private void CheckSystemState(OnSetupVRReady info)
        {
            _setupDone = false;
        }
    }
}
