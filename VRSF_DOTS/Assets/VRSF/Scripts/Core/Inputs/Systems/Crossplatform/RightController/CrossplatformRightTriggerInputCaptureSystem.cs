using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the trigger inputs for the right controllers
    /// </summary>
    public class CrossplatformRightTriggerInputCaptureSystem : JobComponentSystem
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

        [Unity.Burst.BurstCompile]
        struct TriggerInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public float TriggerSqueezeValue;

            public void Execute(ref CrossplatformInputCapture crossplatformInput)
            {
                // Check Click Events
                if (!RightInputsParameters.TriggerClick && TriggerSqueezeValue > crossplatformInput.SqueezeClickThreshold)
                {
                    RightInputsParameters.TriggerClick  = true;
                    RightInputsParameters.TriggerTouch = false;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                }
                else if (RightInputsParameters.TriggerClick && TriggerSqueezeValue < crossplatformInput.SqueezeClickThreshold)
                {
                    RightInputsParameters.TriggerClick = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                }
                // Check Touch Events if user is not clicking
                else if (!RightInputsParameters.TriggerTouch && TriggerSqueezeValue > 0.0f)
                {
                    RightInputsParameters.TriggerTouch = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                }
                else if (RightInputsParameters.TriggerTouch && TriggerSqueezeValue == 0.0f)
                {
                    RightInputsParameters.TriggerTouch = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
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
