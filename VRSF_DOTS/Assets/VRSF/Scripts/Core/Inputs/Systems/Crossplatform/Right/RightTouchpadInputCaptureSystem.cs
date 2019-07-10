using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the touchpad inputs for the right controller
    /// </summary>
    public class RightTouchpadInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new TouchpadInputCaptureJob()
            {
                ThumbPosition = new float2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")),
                RightThumbClickDown = Input.GetButtonDown("RightThumbClick"),
                RightThumbClickUp = Input.GetButtonUp("RightThumbClick"),
                RightThumbTouchDown = Input.GetButtonDown("RightThumbTouch"),
                RightThumbTouchUp = Input.GetButtonUp("RightThumbTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct TouchpadInputCaptureJob : IJobForEach<TouchpadInputCapture, BaseInputCapture>
        {
            public float2 ThumbPosition;

            public bool RightThumbClickDown;
            public bool RightThumbClickUp;

            public bool RightThumbTouchDown;
            public bool RightThumbTouchUp;

            public void Execute(ref TouchpadInputCapture touchpadInput, ref BaseInputCapture baseInput)
            {
                // This system only works for the right controller, as the right input are given as parameters of this system
                if (touchpadInput.Hand == EHand.RIGHT)
                {
                    touchpadInput.ThumbPosition = ThumbPosition;

                    // Check Click Events
                    if (RightThumbClickDown)
                    {
                        baseInput.IsClicking = true;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
                    }
                    else if (RightThumbClickUp)
                    {
                        baseInput.IsClicking = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
                    }
                    // Check Touch Events if user is not clicking
                    else if (!baseInput.IsClicking && RightThumbTouchDown)
                    {
                        baseInput.IsTouching = true;
                        new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
                    }
                    else if (RightThumbTouchUp)
                    {
                        baseInput.IsTouching = false;
                        new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
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
