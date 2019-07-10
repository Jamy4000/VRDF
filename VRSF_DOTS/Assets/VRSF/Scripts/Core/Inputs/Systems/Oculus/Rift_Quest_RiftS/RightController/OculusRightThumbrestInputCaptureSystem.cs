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
    public class OculusRightThumbrestInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new ThumbrestButtonInputCaptureJob()
            {
                ThumbrestTouchButtonDown = Input.GetButtonDown("OculusRightThumbrestTouch"),
                ThumbrestTouchButtonUp = Input.GetButtonUp("OculusRightThumbrestTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct ThumbrestButtonInputCaptureJob : IJobForEach<ThumbrestInputCapture, BaseInputCapture>
        {
            public bool ThumbrestTouchButtonDown;
            public bool ThumbrestTouchButtonUp;

            public void Execute(ref ThumbrestInputCapture thumbrestCapture, ref BaseInputCapture baseInput)
            {
                if (thumbrestCapture.Hand == EHand.RIGHT)
                {
                    if (ThumbrestTouchButtonDown)
                    {
                        baseInput.IsTouching = true;
                        new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
                    }
                    else if (ThumbrestTouchButtonUp)
                    {
                        baseInput.IsTouching = false;
                        new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
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
            this.Enabled = IsOculusHeadset();

            bool IsOculusHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
            }
        }
        #endregion PRIVATE_METHODS
    }
}