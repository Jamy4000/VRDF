using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture inputs for the Right controller of the HTC Vive and Focus
    /// </summary>
    public class HtcRightControllersInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreate();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new MenuInputCaptureJob()
            {
                MenuButtonDown = Input.GetButtonDown("HtcRightMenuClick"),
                MenuButtonUp = Input.GetButtonUp("HtcRightMenuClick")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        #region PRIVATE_METHODS
        [Unity.Burst.BurstCompile]
        [RequireComponentTag(typeof(HtcControllersInputCaptureComponent))]
        struct MenuInputCaptureJob : IJobForEach<CrossplatformInputCapture>
        {
            public bool MenuButtonDown;
            public bool MenuButtonUp;

            public void Execute(ref CrossplatformInputCapture c0)
            {
                // Check Click Events
                if (MenuButtonDown)
                {
                    RightInputsParameters.MenuClick = true;
                    new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
                }
                else if (MenuButtonUp)
                {
                    RightInputsParameters.MenuClick = false;
                    new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
                }
            }
        }


        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE || VRSF_Components.DeviceLoaded == EDevice.HTC_FOCUS;
        }
        #endregion PRIVATE_METHODS
    }
}