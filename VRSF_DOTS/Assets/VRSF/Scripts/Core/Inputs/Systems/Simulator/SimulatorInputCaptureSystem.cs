using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// System to capture some keys from the simulator
    /// </summary>
    public class SimulatorInputCaptureSystem : ComponentSystem
    {
        /// <summary>
        /// The filter for the entity component.
        /// </summary>
        struct Filter
        {
            public CrossplatformInputCapture CrossplatformInput;
            public SimulatorInputCaptureComponent ControllersInputCapture;
        }

        #region PRIVATE_VARIABLES
        // VRSF Parameters references
        private InputVariableContainer _inputContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        /// <summary>
        /// Called after the scene was loaded, setup the entities variables
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            OnSetupVRReady.Listeners += CheckDevice;
            _inputContainer = InputVariableContainer.Instance;
            base.OnCreateManager();
        }

        protected override void OnUpdate()
        {
            foreach (var e in GetEntities<Filter>())
            {
                if (e.CrossplatformInput.IsSetup)
                    CheckRightControllerInput(e.CrossplatformInput);
            }
        }

        protected override void OnDestroyManager()
        {
            OnSetupVRReady.Listeners -= CheckDevice;
            base.OnDestroyManager();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Handle the Right Controller input and put them in the Events
        /// </summary>
        void CheckRightControllerInput(CrossplatformInputCapture inputCapture)
        {
            // Left click
            #region TRIGGER
            bool leftClickIsDown = Input.GetMouseButton(0);
            inputCapture.RightParameters.TriggerSqueezeValue.SetValue(leftClickIsDown ? 1 : 0);
            inputCapture.LeftParameters.TriggerSqueezeValue.SetValue(leftClickIsDown ? 1 : 0);

            // Check Click Events
            if (!inputCapture.RightParameters.TriggerClick.Value && leftClickIsDown)
            {
                inputCapture.RightParameters.TriggerClick.SetValue(true);
                inputCapture.RightParameters.TriggerTouch.SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            else if (inputCapture.RightParameters.TriggerClick.Value && !leftClickIsDown)
            {
                inputCapture.RightParameters.TriggerClick.SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.TRIGGER);
            }
            #endregion TRIGGER

            // Esc 
            #region MENU
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                inputCapture.RightParameters.ClickBools.Get("MenuIsDown").SetValue(true);
                new ButtonClickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                inputCapture.RightParameters.ClickBools.Get("MenuIsDown").SetValue(false);
                new ButtonUnclickEvent(EHand.RIGHT, EControllersButton.MENU);
            }
            #endregion MENU
        }

        private void CheckDevice(OnSetupVRReady info)
        {
            this.Enabled = VRSF_Components.DeviceLoaded == EDevice.SIMULATOR;
        }
        #endregion
    }
}