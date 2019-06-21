using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the VR Headsets, capture the touchpad inputs for the left controllers
    /// </summary>
    public class CrossplatformLeftTouchpadInputCaptureSystem : JobComponentSystem
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
                ThumbPosition = new float2(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft")),
                LeftThumbClickDown = Input.GetButtonDown("LeftThumbClick"),
                LeftThumbClickUp = Input.GetButtonUp("LeftThumbClick"),
                LeftThumbTouchDown = Input.GetButtonDown("LeftThumbTouch"),
                LeftThumbTouchUp = Input.GetButtonUp("LeftThumbTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct TouchpadInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public float2 ThumbPosition;

            public bool LeftThumbClickDown;
            public bool LeftThumbClickUp;

            public bool LeftThumbTouchDown;
            public bool LeftThumbTouchUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                LeftInputsParameters.ThumbPosition = ThumbPosition;

                // Check Click Events
                if (LeftThumbClickDown)
                {
                    LeftInputsParameters.TouchpadClick = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
                }
                else if (LeftThumbClickUp)
                {
                    LeftInputsParameters.TouchpadClick = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
                }
                // Check Touch Events if user is not clicking
                else if (!LeftInputsParameters.TouchpadClick && LeftThumbTouchDown)
                {
                    LeftInputsParameters.TouchpadTouch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
                }
                else if (LeftThumbTouchUp)
                {
                    LeftInputsParameters.TouchpadTouch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
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
