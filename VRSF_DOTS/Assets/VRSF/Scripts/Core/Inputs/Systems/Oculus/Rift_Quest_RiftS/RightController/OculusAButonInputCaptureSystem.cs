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

        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct AButtonInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public bool AClickButtonDown;
            public bool AClickButtonUp;

            public bool ATouchButtonDown;
            public bool ATouchButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (AClickButtonDown)
                {
                    RightInputsParameters.A_Click = true;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
                }
                else if (AClickButtonUp)
                {
                    RightInputsParameters.A_Click = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!RightInputsParameters.A_Click && ATouchButtonDown)
                {
                    RightInputsParameters.A_Touch = true;
                    new ButtonTouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
                }
                else if (ATouchButtonUp)
                {
                    RightInputsParameters.A_Touch = false;
                    new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
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
