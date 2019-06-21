using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    public class WMRRightControllerInputCaptureSystem : JobComponentSystem
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
                MenuButtonDown = Input.GetButtonDown("WMRRightMenuClick"),
                MenuButtonUp = Input.GetButtonUp("WMRRightMenuClick")
            }.Schedule(this, inputDeps);
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroy();
        }

        [RequireComponentTag(typeof(WMRControllersInputCaptureComponent))]
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
        
        #region PRIVATE_METHODS
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.WMR;
        }
        #endregion PRIVATE_METHODS
    }
}