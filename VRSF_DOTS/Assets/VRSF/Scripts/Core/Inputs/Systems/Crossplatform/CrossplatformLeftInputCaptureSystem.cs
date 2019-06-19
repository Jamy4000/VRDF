using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System common for the Simulator, Vive and Rift, capture the basic inputs for the left controllers
    /// </summary>
    public class CrossplatformLeftInputCaptureSystem : ComponentSystem
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
                    // We check the Input for the Left controller
                    CheckLeftControllerInput(e.InputCapture);
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
        private void CheckLeftControllerInput(CrossplatformInputCapture inputCapture)
        {
            #region TRIGGER
            inputCapture.LeftParameters.TriggerSqueezeValue.SetValue(Input.GetAxis("LeftTriggerSqueeze"));
            var triggerSqueezeValue = inputCapture.LeftParameters.TriggerSqueezeValue.Value;

            // Check Click Events
            if (!inputCapture.LeftParameters.TriggerClick.Value && triggerSqueezeValue > 0.95f)
            {
                inputCapture.LeftParameters.TriggerClick.SetValue(true);
                inputCapture.LeftParameters.TriggerTouch.SetValue(false);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            else if (inputCapture.LeftParameters.TriggerClick.Value && triggerSqueezeValue < 0.95f)
            {
                inputCapture.LeftParameters.TriggerClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.LeftParameters.TriggerTouch.Value && triggerSqueezeValue > 0.0f)
            {
                inputCapture.LeftParameters.TriggerTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            else if (inputCapture.LeftParameters.TriggerTouch.Value && triggerSqueezeValue == 0.0f)
            {
                inputCapture.LeftParameters.TriggerTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGER

            #region TOUCHPAD
            inputCapture.LeftParameters.ThumbPosition.SetValue(new Vector2(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft")));

            // Check Click Events
            if (Input.GetButtonDown("LeftThumbClick"))
            {
                inputCapture.LeftParameters.TouchpadClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
            }
            else if (Input.GetButtonUp("LeftThumbClick"))
            {
                inputCapture.LeftParameters.TouchpadClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.LeftParameters.TouchpadClick.Value && Input.GetButtonDown("LeftThumbTouch"))
            {
                inputCapture.LeftParameters.TouchpadTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
            }
            else if (Input.GetButtonUp("LeftThumbTouch"))
            {
                inputCapture.LeftParameters.TouchpadTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.TOUCHPAD);
            }
            #endregion TOUCHPAD

            #region GRIP
            inputCapture.LeftParameters.GripSqueezeValue.SetValue(Input.GetAxis("LeftGripSqueeze"));
            var gripSqueezeValue = inputCapture.LeftParameters.GripSqueezeValue.Value;

            // Check Click Events
            if (!inputCapture.LeftParameters.GripClick.Value && gripSqueezeValue > 0.95f)
            {
                inputCapture.LeftParameters.GripClick.SetValue(true);
                new ButtonClickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            else if (inputCapture.LeftParameters.GripClick.Value && gripSqueezeValue < 0.95f)
            {
                inputCapture.LeftParameters.GripClick.SetValue(false);
                new ButtonUnclickEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            // Check Touch Events if user is not clicking
            else if (!inputCapture.LeftParameters.GripTouch.Value && gripSqueezeValue > 0.0f)
            {
                inputCapture.LeftParameters.GripTouch.SetValue(true);
                new ButtonTouchEvent(EHand.LEFT, EControllersButton.GRIP);
            }
            else if (inputCapture.LeftParameters.GripTouch.Value && gripSqueezeValue == 0.0f)
            {
                inputCapture.LeftParameters.GripTouch.SetValue(false);
                new ButtonUntouchEvent(EHand.LEFT, EControllersButton.GRIP);
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
