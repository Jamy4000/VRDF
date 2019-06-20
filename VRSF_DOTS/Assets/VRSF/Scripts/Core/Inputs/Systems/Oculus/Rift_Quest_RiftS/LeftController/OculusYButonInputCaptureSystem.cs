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
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var job = new YButtonInputCapture()
            {
                YClickButtonDown = Input.GetButtonDown("OculusYButtonClick"),
                YClickButtonUp = Input.GetButtonUp("OculusYButtonClick"),
                YTouchButtonDown = Input.GetButtonDown("OculusYButtonTouch"),
                YTouchButtonUp = Input.GetButtonUp("OculusYButtonTouch")
            };

            return job.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        [RequireComponentTag(typeof(OculusControllersInputCaptureComponent))]
        struct YButtonInputCapture : IJobForEach<CrossplatformInputCapture>
        {
            public bool YClickButtonDown;
            public bool YClickButtonUp;

            public bool YTouchButtonDown;
            public bool YTouchButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (YClickButtonDown)
                {
                    LeftInputsParameters.Y_Click = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                else if (YClickButtonUp)
                {
                    LeftInputsParameters.Y_Click = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                // Check Touch Events if user is not clicking
                else if (!LeftInputsParameters.Y_Click && YTouchButtonDown)
                {
                    LeftInputsParameters.Y_Touch = true;
                    new ButtonTouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
                }
                else if (YTouchButtonUp)
                {
                    LeftInputsParameters.Y_Touch = false;
                    new ButtonUntouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
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