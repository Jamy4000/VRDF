using System.ComponentModel;
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
    public class SimulatorEscapeButtonInputCaptureSystem : JobComponentSystem
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
                EscapeButtonWasClick = Input.GetKeyDown(KeyCode.Escape)
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }
        #endregion

        struct SimulatorInputCaptureJob : IJobForEach<MenuInputCapture, BaseInputCapture>
        {
            public bool EscapeButtonWasClick;

            public void Execute(ref MenuInputCapture menuInput, ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (!baseInput.IsClicking && EscapeButtonWasClick)
                {
                    baseInput.IsClicking = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
                }
                else if (baseInput.IsClicking && !EscapeButtonWasClick)
                {
                    baseInput.IsClicking = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
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