using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusYButonInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new YButtonInputCaptureJob()
            {
                YClickButtonDown = Input.GetButtonDown("OculusYButtonClick"),
                YClickButtonUp = Input.GetButtonUp("OculusYButtonClick"),
                YTouchButtonDown = Input.GetButtonDown("OculusYButtonTouch"),
                YTouchButtonUp = Input.GetButtonUp("OculusYButtonTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct YButtonInputCaptureJob : IJobForEach<YButtonInputCapture>
        {
            public bool YClickButtonDown;
            public bool YClickButtonUp;

            public bool YTouchButtonDown;
            public bool YTouchButtonUp;

            public void Execute(ref YButtonInputCapture yButtonInput)
            {
                // Check Click Events
                if (YClickButtonDown)
                {
                    yButtonInput.Y_Click = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                else if (YClickButtonUp)
                {
                    yButtonInput.Y_Click = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!yButtonInput.Y_Click && YTouchButtonDown)
                {
                    yButtonInput.Y_Touch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                else if (YTouchButtonUp)
                {
                    yButtonInput.Y_Touch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
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
            this.Enabled = IsOculusHeadset();

            bool IsOculusHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
            }
        }
        #endregion PRIVATE_METHODS
    }
}