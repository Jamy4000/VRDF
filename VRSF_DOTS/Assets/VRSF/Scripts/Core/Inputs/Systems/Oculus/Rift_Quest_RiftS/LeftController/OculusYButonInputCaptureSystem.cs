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

        [RequireComponentTag(typeof(YButtonInputCapture))]
        struct YButtonInputCaptureJob : IJobForEach<BaseInputCapture>
        {
            public bool YClickButtonDown;
            public bool YClickButtonUp;

            public bool YTouchButtonDown;
            public bool YTouchButtonUp;

            public void Execute(ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (YClickButtonDown)
                {
                    baseInput.IsClicking = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                else if (YClickButtonUp)
                {
                    baseInput.IsClicking = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && YTouchButtonDown)
                {
                    baseInput.IsTouching = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                else if (YTouchButtonUp)
                {
                    baseInput.IsTouching = false;
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