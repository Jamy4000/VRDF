using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the Simulator, Vive and Rift, capture the basic inputs for the right controllers
    /// </summary>
    public class CrossplatformRightInputCaptureSystem : ComponentSystem
    {
        private struct Filter
        {
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
                    // We check the Input for the Right controller
                    CheckRightControllerInput(e.InputCapture);
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
        private void CheckRightControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region TRIGGER
            inputCapture.RightParameters.TriggerSqueezeValue.SetValue(Input.GetAxis("RightTriggerSqueeze"));
            var triggerSqueezeValue = inputCapture.RightParameters.TriggerSqueezeValue.Value;
            
            // Check Click Events
            if (!inputCapture.RightParameters.TriggerClick.Value && triggerSqueezeValue > 0.95f)
            {
                inputCapture.RightParameters.TriggerClick.SetValue(true);
                inputCapture.RightParameters.TriggerTouch.SetValue(false);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (inputCapture.RightParameters.TriggerClick.Value && triggerSqueezeValue < 0.95f)
            {
                inputCapture.RightParameters.TriggerClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.RightParameters.TriggerTouch.Value && triggerSqueezeValue > 0.0f)
            {
                inputCapture.RightParameters.TriggerTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (inputCapture.RightParameters.TriggerTouch.Value && triggerSqueezeValue == 0.0f)
            {
                inputCapture.RightParameters.TriggerTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGER

            #region TOUCHPAD
            inputCapture.RightParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight")));
            
            // Check Click Events
            if (Input.GetButtonDown("RightThumbClick"))
            {
                inputCapture.RightParameters.TouchpadClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
            }
            else if (Input.GetButtonUp("RightThumbClick"))
            {
                inputCapture.RightParameters.TouchpadClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.RightParameters.TouchpadClick.Value && Input.GetButtonDown("RightThumbTouch"))
            {
                inputCapture.RightParameters.TouchpadTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
            }
            else if (Input.GetButtonUp("RightThumbTouch"))
            {
                inputCapture.RightParameters.TouchpadTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.TOUCHPAD);
            }
            #endregion TOUCHPAD

            #region GRIP
            inputCapture.RightParameters.GripSqueezeValue.SetValue(Input.GetAxis("RightGripSqueeze"));
            var gripSqueezeValue = inputCapture.RightParameters.GripSqueezeValue.Value;
            
            // Check Click Events
            if (!inputCapture.RightParameters.GripClick.Value && gripSqueezeValue > 0.95f)
            {
                inputCapture.RightParameters.GripClick.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            else if (inputCapture.RightParameters.GripClick.Value && gripSqueezeValue < 0.95f)
            {
                inputCapture.RightParameters.GripClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.RightParameters.GripTouch.Value && gripSqueezeValue > 0.0f)
            {
                inputCapture.RightParameters.GripTouch.SetValue(true);
                new ButtonTouchEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            else if (inputCapture.RightParameters.GripTouch.Value && gripSqueezeValue == 0.0f)
            {
                inputCapture.RightParameters.GripTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.RIGHT, EControllersButton.GRIP);
            }
            #endregion GRIP
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded != EDevice.SIMULATOR;
        }
        #endregion PRIVATE_METHODS
    }
}