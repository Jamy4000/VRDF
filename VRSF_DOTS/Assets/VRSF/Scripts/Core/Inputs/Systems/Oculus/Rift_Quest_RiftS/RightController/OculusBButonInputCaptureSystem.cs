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

        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct BButtonInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public bool BClickButtonDown;
            public bool BClickButtonUp;

            public bool BTouchButtonDown;
            public bool BTouchButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (BClickButtonDown)
                {
                    RightInputsParameters.B_Click = true;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
                }
                else if (BClickButtonUp)
                {
                    RightInputsParameters.B_Click = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!RightInputsParameters.B_Click && BTouchButtonDown)
                {
                    RightInputsParameters.B_Touch = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
                }
                else if (BTouchButtonUp)
                {
                    RightInputsParameters.B_Touch = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
                }
            }
        }

        #region PRIVATE_METHODS
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