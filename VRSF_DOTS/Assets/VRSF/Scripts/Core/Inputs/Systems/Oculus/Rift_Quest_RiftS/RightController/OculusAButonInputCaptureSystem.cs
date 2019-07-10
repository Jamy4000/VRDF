using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture the inputs for the A Button on the right controller of the Oculus Rift, Rift S and QUest
    /// </summary>
    public class OculusAButonInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new AButtonInputCaptureJob()
            {
                AClickButtonDown = Input.GetButtonDown("OculusAButtonClick"),
                AClickButtonUp = Input.GetButtonUp("OculusAButtonClick"),
                ATouchButtonDown = Input.GetButtonDown("OculusAButtonTouch"),
                ATouchButtonUp = Input.GetButtonUp("OculusAButtonTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [RequireComponentTag(typeof(AButtonInputCapture))]
        struct AButtonInputCaptureJob : IJobForEach<BaseInputCapture>
        {
            public bool AClickButtonDown;
            public bool AClickButtonUp;

            public bool ATouchButtonDown;
            public bool ATouchButtonUp;

            public void Execute(ref BaseInputCapture baseInput)
            {
                // Check Click Events
                if (AClickButtonDown)
                {
                    baseInput.IsClicking = true;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
                }
                else if (AClickButtonUp)
                {
                    baseInput.IsClicking = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!baseInput.IsClicking && ATouchButtonDown)
                {
                    baseInput.IsTouching = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
                }
                else if (ATouchButtonUp)
                {
                    baseInput.IsTouching = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
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
