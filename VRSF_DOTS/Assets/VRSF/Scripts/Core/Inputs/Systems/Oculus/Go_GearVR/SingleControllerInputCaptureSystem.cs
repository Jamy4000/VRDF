using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Only capturing the Back Button on the GearVR and the Oculus Go controller
    /// </summary>
    public class SignleControllerInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
            public GoAndGearVRControllersInputCaptureComponent SingleControllerInput;
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
                    CheckControllerInput(e.SingleControllerInput);
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
        private void CheckControllerInput(GoAndGearVRControllersInputCaptureComponent inputCapture)
        {
            #region BACK
            // Check Click Events
            if (Input.GetButtonDown("BackButtonClick"))
            {
                inputCapture.BackButtonClick.SetValue(true);
                inputCapture.BackButtonClick.SetValue(false);
                new ButtonClickEvent(inputCapture.IsUserRightHanded ? EHand.RIGHT : EHand.LEFT, EControllersButton.BACK_BUTTON);
            }
            else if (Input.GetButtonUp("BackButtonClick"))
            {
                inputCapture.BackButtonClick.SetValue(false);
                new ButtonUnclickEvent(inputCapture.IsUserRightHanded ? EHand.RIGHT : EHand.LEFT, EControllersButton.BACK_BUTTON);
            }
            #endregion BACK
        }
        
        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = IsSingleControllerHeadset();
            
            bool IsSingleControllerHeadset()
            {
                return VRSF_Components.DeviceLoaded == EDevice.GEAR_VR || VRSF_Components.DeviceLoaded == EDevice.OCULUS_GO;
            }
        }
        #endregion PRIVATE_METHODS
    }
}
