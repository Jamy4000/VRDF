using UnityEngine;
using Unity.Entities;
using VRSF.Core.SetupVR;
using Unity.Jobs;
using Unity.Collections;

namespace VRSF.Core.Simulator
{
    public class SimulatorBoostSystem : JobComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.RegisterSetupVRResponse(CheckSystemState);
            base.OnCreateManager();
        }
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new SimulatorBoostJob
            {
                WheelScroll = Input.GetAxis("Mouse Scrollwheel")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            if (OnSetupVRReady.IsMethodAlreadyRegistered(CheckSystemState))
                OnSetupVRReady.Listeners -= CheckSystemState;
        }

        private struct SimulatorBoostJob : IJobForEach<SimulatorMovementSpeed>
        {
            public float WheelScroll;

            public void Execute(ref SimulatorMovementSpeed sms)
            {
                // Check for mouse scroll wheel going up or down, and set shift boost based on that
                if (WheelScroll > 0)
                    sms.LeftShiftBoost += 0.2f;
                else if (WheelScroll < 0)
                    sms.LeftShiftBoost -= 0.2f;
            }
        }


        private void CheckSystemState(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
    }
}
