using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the left controller
    /// </summary>
    public class LeftGripInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new GripInputCaptureJob()
            {
                GripSqueezeValue = Input.GetAxis("LeftGripSqueeze")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [Unity.Burst.BurstCompile]
        struct GripInputCaptureJob : IJobForEach<GripInputCapture>
        {
            public float GripSqueezeValue;

            public void Execute(ref GripInputCapture gripInput)
            {
                // This system only works for the left controller, as the left input are given as parameters of this system
                if (gripInput.Hand == EHand.LEFT)
                {
                    // Check Click Events
                    if (!gripInput.GripClick && GripSqueezeValue > gripInput.SqueezeClickThreshold)
                    {
                        gripInput.GripClick = true;
                        gripInput.GripTouch = false;
                        new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
                    }
                    else if (gripInput.GripClick && GripSqueezeValue < gripInput.SqueezeClickThreshold)
                    {
                        gripInput.GripClick = false;
                        new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!gripInput.GripTouch && GripSqueezeValue > 0.0f)
                    {
                        gripInput.GripTouch = true;
                        new ButtonTouchEvent(EHand.LEFT, EControllersButton.GRIP);
                    }
                    else if (gripInput.GripTouch && GripSqueezeValue == 0.0f)
                    {
                        gripInput.GripTouch = false;
                        new ButtonUntouchEvent(EHand.LEFT, EControllersButton.GRIP);
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
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR && VRSF_Components.DeviceLoaded != EDevice.GEAR_VR && VRSF_Components.DeviceLoaded != EDevice.OCULUS_GO;
        }
        #endregion PRIVATE_METHODS
    }
}
