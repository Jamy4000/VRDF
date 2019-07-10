using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the right controller
    /// </summary>
    public class RightTriggerInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TriggerInputCaptureJob
            {
                TriggerSqueezeValue = Input.GetAxis("RightTriggerSqueeze")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct TriggerInputCaptureJob : IJobForEach<TriggerInputCapture, BaseInputCapture>
        {
            public float TriggerSqueezeValue;

            public void Execute(ref TriggerInputCapture triggerInput, ref BaseInputCapture baseInput)
            {
                // This system only works for the right controller, as the right input are given as parameters of this system
                if (triggerInput.Hand == EHand.RIGHT)
                {
                    // Check Click Events
                    if (!baseInput.IsClicking && TriggerSqueezeValue > triggerInput.SqueezeClickThreshold)
                    {
                        baseInput.IsClicking = true;
                        baseInput.IsTouching= false;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                    }
                    else if (baseInput.IsClicking && TriggerSqueezeValue < triggerInput.SqueezeClickThreshold)
                    {
                        baseInput.IsClicking = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!baseInput.IsTouching && TriggerSqueezeValue > 0.0f)
                    {
                        baseInput.IsTouching = true;
                        new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                    }
                    else if (baseInput.IsTouching && TriggerSqueezeValue == 0.0f)
                    {
                        baseInput.IsTouching = false;
                        new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
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
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
        #endregion PRIVATE_METHODS
    }
}
