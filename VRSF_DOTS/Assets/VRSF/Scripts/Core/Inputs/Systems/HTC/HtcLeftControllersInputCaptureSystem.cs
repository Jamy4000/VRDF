using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture inputs for the Left controller of the HTC Vive and Focus
    /// </summary>
    public class HtcLeftControllersInputCaptureSystem : JobComponentSystem
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
                MenuButtonDown = Input.GetButtonDown("HtcLeftMenuClick"),
                MenuButtonUp = Input.GetButtonUp("HtcLeftMenuClick")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

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
                    LeftInputsParameters.MenuClick = true;
                    new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
                }
                else if (MenuButtonUp)
                {
                    LeftInputsParameters.MenuClick = false;
                    new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
                }
            }
        }

        #region PRIVATE_METHODS
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE || VRSF_Components.DeviceLoaded == EDevice.HTC_FOCUS;
        }
        #endregion PRIVATE_METHODS
    }
}