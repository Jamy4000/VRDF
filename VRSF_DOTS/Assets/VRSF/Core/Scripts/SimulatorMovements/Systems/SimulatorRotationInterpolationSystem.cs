using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Simulator
{
    [UpdateAfter(typeof(SimulatorRotationSystem))]
    public class SimulatorRotationInterpolationSystem : ComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.RegisterSetupVRResponse(CheckSystemState);
            base.OnCreateManager();
        }
        
        protected override void OnUpdate()
        {
            if (VRSF_Components.SetupVRIsReady)
            {
                Entities.ForEach((ref SimulatorMovementRotation smr, ref SimulatorMovementSpeed sms, ref SimulatorCameraState scs) =>
                {
                    // Interpolate toward new position
                    if (Input.GetMouseButton(1))
                        SimulatorMovementSystem.Interpolate(sms, smr, ref scs, Time.DeltaTime);
                });
            }
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(CheckSystemState))
                OnSetupVRReady.Listeners -= CheckSystemState;
        }

        private void CheckSystemState(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}
