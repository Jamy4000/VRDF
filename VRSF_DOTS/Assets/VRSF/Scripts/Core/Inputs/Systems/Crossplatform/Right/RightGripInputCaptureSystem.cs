using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the grip inputs for the right controller
    /// </summary>
    public class RightGripInputCaptureSystem : JobComponentSystem
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
                GripSqueezeValue = Input.GetAxis("RightGripSqueeze")
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
                // This system only works for the right controller, as the right input are given as parameters of this system
                if (gripInput.Hand == EHand.RIGHT)
                {
                    // Check Click Events
                    if (!gripInput.GripClick && GripSqueezeValue > gripInput.SqueezeClickThreshold)
                    {
                        gripInput.GripClick = true;
                        gripInput.GripTouch = false;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
                    }
                    else if (gripInput.GripClick && GripSqueezeValue < gripInput.SqueezeClickThreshold)
                    {
                        gripInput.GripClick = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!gripInput.GripTouch && GripSqueezeValue > 0.0f)
                    {
                        gripInput.GripTouch = true;
                        new ButtonTouchEvent(EHand.RIGHT, EControllersButton.GRIP);
                    }
                    else if (gripInput.GripTouch && GripSqueezeValue == 0.0f)
                    {
                        gripInput.GripTouch = false;
                        new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.GRIP);
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
