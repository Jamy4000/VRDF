using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the left controllers
    /// </summary>
    public class CrossplatformLeftTriggerInputCaptureSystem : JobComponentSystem
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
                TriggerSqueezeValue = Input.GetAxis("LeftTriggerSqueeze")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct TriggerInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public float TriggerSqueezeValue;

            public void Execute(ref CrossplatformInputCapture crossplatformInput)
            {
                // Check Click Events
                if (!LeftInputsParameters.TriggerClick && TriggerSqueezeValue > crossplatformInput.SqueezeClickThreshold)
                {
                    LeftInputsParameters.TriggerClick  = true;
                    LeftInputsParameters.TriggerTouch = false;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                }
                else if (LeftInputsParameters.TriggerClick && TriggerSqueezeValue < crossplatformInput.SqueezeClickThreshold)
                {
                    LeftInputsParameters.TriggerClick = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                }
                // Check Touch Events if user is not clicking
                else if (!LeftInputsParameters.TriggerTouch && TriggerSqueezeValue > 0.0f)
                {
                    LeftInputsParameters.TriggerTouch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
                }
                else if (LeftInputsParameters.TriggerTouch && TriggerSqueezeValue == 0.0f)
                {
                    LeftInputsParameters.TriggerTouch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
                }
            }
        }

        #region PRIVATE_METHODS
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
        #endregion PRIVATE_METHODS
    }
}
