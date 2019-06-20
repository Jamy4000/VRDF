using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the right controllers
    /// </summary>
    public class CrossplatformRightGripInputCaptureSystem : JobComponentSystem
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
                GripSqueezeValue = Input.GetAxis("RightGripSqueeze")
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
                if (!RightInputsParameters.GripClick && GripSqueezeValue > 0.95f)
                {
                    RightInputsParameters.GripClick = true;
                    RightInputsParameters.GripTouch = false;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
                }
                else if (RightInputsParameters.GripClick && GripSqueezeValue < 0.95f)
                {
                    RightInputsParameters.GripClick = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
                }
                // Check Touch Events if user is not clicking
                else if (!RightInputsParameters.GripTouch && GripSqueezeValue > 0.0f)
                {
                    RightInputsParameters.GripTouch = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.GRIP);
                }
                else if (RightInputsParameters.GripTouch && GripSqueezeValue == 0.0f)
                {
                    RightInputsParameters.GripTouch = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.GRIP);
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
