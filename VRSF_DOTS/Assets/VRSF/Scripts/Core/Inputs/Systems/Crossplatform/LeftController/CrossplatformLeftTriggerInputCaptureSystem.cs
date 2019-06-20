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
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new TriggerInputCapture
            {
                TriggerSqueezeValue = Input.GetAxis("LeftTriggerSqueeze")
            };

            return job.Schedule(this, inputDeps);
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
                if (!LeftInputsParameters.TriggerClick && TriggerSqueezeValue > 0.95f)
                {
                    LeftInputsParameters.TriggerClick  = true;
                    LeftInputsParameters.TriggerTouch = false;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
                }
                else if (LeftInputsParameters.TriggerClick && TriggerSqueezeValue < 0.95f)
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
