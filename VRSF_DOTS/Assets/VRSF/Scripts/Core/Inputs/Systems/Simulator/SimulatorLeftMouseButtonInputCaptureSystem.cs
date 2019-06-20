using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System to capture some keys from the simulator
    /// </summary>
    public class SimulatorLeftMouseButtonInputCaptureSystem : JobComponentSystem
    {
        #region JobComponentSystem_Methods
        /// <summary>
        /// Called after the scene was loaded, setup the entities variables
        /// </summary>
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var inputCaptureJob = new SimulatorInputCaptureJob()
            {
                LeftMouseButtonIsClicking = Input.GetMouseButton(0)
            };

            return inputCaptureJob.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }
        #endregion


        [RequireComponentTag(typeof(SimulatorInputCaptureComponent))]
        struct SimulatorInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public bool LeftMouseButtonIsClicking;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                LeftInputsParameters.TriggerSqueezeValue = LeftMouseButtonIsClicking ? 1 : 0;

                // Check Click Events
                if (!LeftInputsParameters.TriggerClick && LeftMouseButtonIsClicking)
                {
                    LeftInputsParameters.TriggerClick = true;
                    LeftInputsParameters.TriggerTouch = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                }
                else if (LeftInputsParameters.TriggerClick && !LeftMouseButtonIsClicking)
                {
                    LeftInputsParameters.TriggerClick = false;
                    LeftInputsParameters.TriggerTouch = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                }
            }
        }

        #region PRIVATE_METHODS
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}