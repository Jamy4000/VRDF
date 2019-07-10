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

        struct MenuInputCaptureJob : IJobForEach<MenuInputCapture, BaseInputCapture>
        {
            public bool MenuButtonDown;
            public bool MenuButtonUp;

            public void Execute(ref MenuInputCapture menuInput, ref BaseInputCapture baseInput)
            {
                if (menuInput.Hand == EHand.RIGHT)
                {
                    // Check Click Events
                    if (MenuButtonDown)
                    {
                        baseInput.IsClicking = true;
                        new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
                    }
                    else if (MenuButtonUp)
                    {
                        baseInput.IsClicking = false;
                        new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
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
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.WMR;
        }
        #endregion PRIVATE_METHODS
    }
}