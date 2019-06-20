using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the left controllers
    /// </summary>
    public class CrossplatformLeftGripInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var gripJob = new GripInputCapture()
            {
                GripSqueezeValue = Input.GetAxis("LeftGripSqueeze")
            };

            return gripJob.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        struct GripInputCapture : IJobForEach<CrossplatformInputCapture>
        {
            public float GripSqueezeValue;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (!LeftInputsParameters.GripClick && GripSqueezeValue > 0.95f)
                {
                    LeftInputsParameters.GripClick = true;
                    LeftInputsParameters.GripTouch = false;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
                }
                else if (LeftInputsParameters.GripClick && GripSqueezeValue < 0.95f)
                {
                    LeftInputsParameters.GripClick = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
                }
                // Check Touch Events if user is not clicking
                else if (!LeftInputsParameters.GripTouch && GripSqueezeValue > 0.0f)
                {
                    LeftInputsParameters.GripTouch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.GRIP);
                }
                else if (LeftInputsParameters.GripTouch && GripSqueezeValue == 0.0f)
                {
                    LeftInputsParameters.GripTouch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.GRIP);
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
