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
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new SimulatorInputCaptureJob()
            {
                LeftMouseButtonIsClicking = Input.GetMouseButton(0)
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }
        #endregion


        struct SimulatorInputCaptureJob : IJobForEach<TriggerInputCapture, BaseInputCapture>
        {
            public bool LeftMouseButtonIsClicking;

            public void Execute(ref TriggerInputCapture triggerInput, ref BaseInputCapture baseInput)
            {
                if (triggerInput.Hand == EHand.LEFT)
                {
                    triggerInput.TriggerSqueezeValue = LeftMouseButtonIsClicking ? 1 : 0;

                    // Check Click Events
                    if (!baseInput.IsClicking && LeftMouseButtonIsClicking)
                    {
                        baseInput.IsClicking = true;
                        baseInput.IsTouching = true;
                        new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                    else if (baseInput.IsClicking && !LeftMouseButtonIsClicking)
                    {
                        baseInput.IsClicking = false;
                        baseInput.IsTouching = false;
                        new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                    }
                }
            }
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Check if we use the good device
        /// </summary>
        /// <param name="info"></param>
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}