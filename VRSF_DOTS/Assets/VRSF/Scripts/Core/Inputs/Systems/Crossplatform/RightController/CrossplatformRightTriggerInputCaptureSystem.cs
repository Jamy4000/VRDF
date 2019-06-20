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
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var triggerJob = new TriggerInputCapture
            {
                TriggerSqueezeValue = Input.GetAxis("RightTriggerSqueeze")
            };

            return triggerJob.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        struct TriggerInputCapture : IJobForEach<CrossplatformInputCapture>
        {
            public float TriggerSqueezeValue;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (!RightInputsParameters.TriggerClick && TriggerSqueezeValue > 0.95f)
                {
                    RightInputsParameters.TriggerClick  = true;
                    RightInputsParameters.TriggerTouch = false;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
                }
                else if (RightInputsParameters.TriggerClick && TriggerSqueezeValue < 0.95f)
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
