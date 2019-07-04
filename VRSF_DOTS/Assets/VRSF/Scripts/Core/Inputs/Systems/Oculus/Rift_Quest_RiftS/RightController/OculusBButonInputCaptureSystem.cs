using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture the inputs for the B Button on the right controller of the Oculus Rift, Rift S and QUest
    /// </summary>
    public class OculusBButonInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new BButtonInputCaptureJob()
            {
                BClickButtonDown = Input.GetButtonDown("OculusBButtonClick"),
                BClickButtonUp = Input.GetButtonUp("OculusBButtonClick"),
                BTouchButtonDown = Input.GetButtonDown("OculusBButtonTouch"),
                BTouchButtonUp = Input.GetButtonUp("OculusBButtonTouch")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        struct BButtonInputCaptureJob : IJobForEach<BButtonInputCapture>
        {
            public bool BClickButtonDown;
            public bool BClickButtonUp;

            public bool BTouchButtonDown;
            public bool BTouchButtonUp;

            public void Execute(ref BButtonInputCapture bButtonCapture)
            {
                // Check Click Events
                if (BClickButtonDown)
                {
                    bButtonCapture.B_Click = true;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
                }
                else if (BClickButtonUp)
                {
                    bButtonCapture.B_Click = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!bButtonCapture.B_Click && BTouchButtonDown)
                {
                    bButtonCapture.B_Touch = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
                }
                else if (BTouchButtonUp)
                {
                    bButtonCapture.B_Touch = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
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