using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusRightControllerInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
            public OculusControllersInputCaptureComponent OculusControllersInput;
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
                    CheckControllerInput(e.OculusControllersInput);
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
        private void CheckControllerInput(OculusControllersInputCaptureComponent inputCapture)
        {
            #region A
            // Check Click Events
            if (Input.GetButtonDown("OculusAButtonClick"))
            {
                inputCapture.AButtonClick.SetValue(true);
                inputCapture.AButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            else if (Input.GetButtonUp("OculusAButtonClick"))
            {
                inputCapture.AButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.AButtonClick.Value && Input.GetButtonDown("OculusAButtonTouch"))
            {
                inputCapture.AButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            else if (Input.GetButtonUp("OculusAButtonTouch"))
            {
                inputCapture.AButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.A_BUTTON);
            }
            #endregion A

            #region B
            // Check Click Events
            if (Input.GetButtonDown("OculusBButtonClick"))
            {
                inputCapture.BButtonClick.SetValue(true);
                inputCapture.BButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            else if (Input.GetButtonUp("OculusBButtonClick"))
            {
                inputCapture.BButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.BButtonClick.Value && Input.GetButtonDown("OculusBButtonTouch"))
            {
                inputCapture.BButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            else if (Input.GetButtonUp("OculusBButtonTouch"))
            {
                inputCapture.BButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.B_BUTTON);
            }
            #endregion B

            #region THUMBREST
            // Check Touch Events
            if (!inputCapture.RightThumbrestTouch.Value && Input.GetButton("OculusRightThumbrestTouch"))
            {
                inputCapture.RightThumbrestTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            }
            else if (Input.GetButtonUp("OculusRightThumbrestTouch"))
            {
                inputCapture.RightThumbrestTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.THUMBREST);
            }
            #endregion THUMBREST
        }
        
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
