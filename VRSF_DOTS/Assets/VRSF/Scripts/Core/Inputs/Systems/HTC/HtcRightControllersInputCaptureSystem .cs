using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Capture inputs for the Right controller of the HTC Vive and Focus
    /// </summary>
    public class HtcRightControllersInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
            public HtcControllersInputCaptureComponent HtcControllersInput;
            public CrossplatformInputCapture InputCapture;
        }

        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.InputCapture.IsSetup)
                {
                    // We check the Input for the Right controller
                    CheckRightControllerInput(e.HtcControllersInput);
                }
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }

        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        private void CheckRightControllerInput(HtcControllersInputCaptureComponent inputCapture)
        {
            #region MENU
            // Check Click Events
            if (Input.GetButtonDown("HtcRightMenuClick"))
            {
                inputCapture.RightMenuClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("HtcRightMenuClick"))
            {
                inputCapture.RightMenuClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            #endregion MENU
        }
        
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE || VRSF_Components.DeviceLoaded == EDevice.HTC_FOCUS;
        }
        #endregion PRIVATE_METHODS
    }
}
