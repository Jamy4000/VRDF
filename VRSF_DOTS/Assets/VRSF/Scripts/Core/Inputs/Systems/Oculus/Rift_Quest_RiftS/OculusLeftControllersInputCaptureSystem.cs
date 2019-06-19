using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// 
    /// </summary>
    public class OculusLeftControllersInputCaptureSystem : ComponentSystem
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
                    // We check the Input for the Left controller
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
        /// Handle the Left Controller input and put them in the Events
        /// </summary>
        private void CheckControllerInput(OculusControllersInputCaptureComponent inputCapture)
        {
            #region X
            // Check Click Events
            if (Input.GetButtonDown("OculusXButtonClick"))
            {
                inputCapture.XButtonClick.SetValue(true);
                inputCapture.XButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            else if (Input.GetButtonUp("OculusXButtonClick"))
            {
                inputCapture.XButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.XButtonClick.Value && Input.GetButtonDown("OculusXButtonTouch"))
            {
                inputCapture.XButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            else if (Input.GetButtonUp("OculusXButtonTouch"))
            {
                inputCapture.XButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.X_BUTTON);
            }
            #endregion X

            #region Y
            // Check Click Events
            if (Input.GetButtonDown("OculusYButtonClick"))
            {
                inputCapture.YButtonClick.SetValue(true);
                inputCapture.YButtonTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            else if (Input.GetButtonUp("OculusYButtonClick"))
            {
                inputCapture.YButtonClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.YButtonClick.Value && Input.GetButtonDown("OculusYButtonTouch"))
            {
                inputCapture.YButtonTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            else if (Input.GetButtonUp("OculusYButtonTouch"))
            {
                inputCapture.YButtonTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.Y_BUTTON);
            }
            #endregion Y

            #region MENU
            // Check Click Events
            if (Input.GetButtonDown("OculusMenuButtonClick"))
            {
                inputCapture.LeftMenuClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            else if (Input.GetButtonUp("OculusMenuButtonClick"))
            {
                inputCapture.LeftMenuClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.MENU);
            }
            #endregion MENU

            #region THUMBREST
            // Check Touch Events
            if (!inputCapture.LeftThumbrestTouch.Value && Input.GetButton("OculusLeftThumbrestTouch"))
            {
                inputCapture.LeftThumbrestTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
            }
            else if (Input.GetButtonUp("OculusLeftThumbrestTouch"))
            {
                inputCapture.LeftThumbrestTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.THUMBREST);
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
