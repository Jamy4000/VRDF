using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    public class WMRRightControllerInputCaptureSystem : JobComponentSystem
    {
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var touchpadJob = new MenuInputCapture()
            {
                MenuButtonDown = Input.GetButtonDown("WMRRightMenuClick"),
                MenuButtonUp = Input.GetButtonUp("WMRRightMenuClick")
            };

            return touchpadJob.Schedule(this, inputDeps);
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        [RequireComponentTag(typeof(WMRControllersInputCaptureComponent))]
        struct MenuInputCapture : IJobForEach<CrossplatformInputCapture>
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
        
        #region PRIVATE_METHODS
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.WMR;
        }
        #endregion PRIVATE_METHODS
    }
}