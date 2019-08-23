using Unity.Entities;
using UnityEngine;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Simulator
{
    public class SimulatorCameraStateSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.RegisterSetupVRResponse(CheckSystemState);
            base.OnCreateManager();
            this.Enabled = false;
        }
        
        protected override void OnUpdate()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Entities.ForEach((ref SimulatorCameraState scs) =>
                {
                    ResetCameraStateTransform(scs);
                });
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(CheckSystemState))
                OnSetupVRReady.Listeners -= CheckSystemState;
        }

        public static void ResetCameraStateTransform(SimulatorCameraState scs)
        {
            scs.InterpolatingCameraState.SetFromTransform(VRSF_Components.CameraRig.transform);
            scs.TargetCameraState.SetFromTransform(VRSF_Components.CameraRig.transform);
        }
        
        private void CheckSystemState(OnSetupVRReady info)
        {
            Entities.ForEach((ref SimulatorCameraState scs) =>
            {
                ResetCameraStateTransform(scs);
            });
        }
    }
}
