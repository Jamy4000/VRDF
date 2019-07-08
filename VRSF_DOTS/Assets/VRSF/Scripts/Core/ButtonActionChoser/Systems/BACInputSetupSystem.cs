using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;
using VRSF.Core.Events;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    /// <summary>
    /// Setup the Action button Parameter that the user has chosen and check the parameters linked to it (Like the thumb position for a Thumbstick button)
    /// </summary>
    public class BACInputSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public BACGeneralComponent BACGeneralComp;
            public BACCalculationsComponent BACCalculationsComp;
        }

        #region PRIVATE_VARIBALES
        private InputVariableContainer _inputsContainer;
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            SDKChoserIsSetup.Listeners += StartBACsSetup;
            _inputsContainer = InputVariableContainer.Instance;

            base.OnCreateManager();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            base.OnDestroyManager();
            SDKChoserIsSetup.Listeners -= StartBACsSetup;
        }
        #endregion


        #region PRIVATES_METHODS
        /// <summary>
        /// We check which hand correspond to the Action Button that was choosen
        /// </summary>
        private void CheckButtonHand(Filter entity)
        {
            switch (entity.BACGeneralComp.ActionButton)
            {
                // if the Action Button is set to the A, B or Right Thumbrest option (OCULUS SPECIFIC)
                case EControllersButton.A_BUTTON:
                case EControllersButton.B_BUTTON:
                case EControllersButton.X_BUTTON:
                case EControllersButton.Y_BUTTON:
                case EControllersButton.THUMBREST:
                    entity.BACCalculationsComp.IsUsingOculusButton = true;
                    break;

                // if the Action Button is set to the Right Menu option (VIVE SPECIFIC)
                case EControllersButton.MENU:
                    entity.BACCalculationsComp.IsUsingViveButton = entity.BACGeneralComp.ButtonHand == EHand.RIGHT;
                    break;
            }
        }


        /// <summary>
        /// Check that all the parameters are set correctly in the Inspector.
        /// </summary>
        /// <returns>false if the parameters are incorrect</returns>
        private bool CheckParameters(Filter entity)
        {
            //Check if the Thumbstick are used, and if they are set correctly in that case.
            if (!CheckGivenThumbParameter(entity))
                return false;

            //Check if the Action Button specified is set correctly
            if (entity.BACCalculationsComp.CorrectSDK && !CheckActionButton(entity))
                return false;
            
            return entity.BACGeneralComp.InteractionType != EControllerInteractionType.NONE && entity.BACGeneralComp.ActionButton != EControllersButton.NONE;
        }


        /// <summary>
        /// Called if the User is using his Thumb for this feature. Check if the Position to use on the thumbstick are set correctly in the Inspector.
        /// </summary>
        /// <returns>true if everything is set correctly</returns>
        private bool CheckGivenThumbParameter(Filter entity)
        {
            if (entity.BACGeneralComp.ActionButton == EControllersButton.TOUCHPAD &&
                entity.BACGeneralComp.ButtonHand == EHand.LEFT)
            {
                if (entity.BACGeneralComp.LeftClickThumbPosition == EThumbPosition.NONE &&
                    entity.BACGeneralComp.LeftTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to assign a Thumb Position for the Left Thumbstick in this script : " + entity.BACGeneralComp.name);
                    return false;
                }

                entity.BACCalculationsComp.ThumbPos = _inputsContainer.LeftThumbPosition;
            }
            else if (entity.BACGeneralComp.ActionButton == EControllersButton.TOUCHPAD &&
                entity.BACGeneralComp.ButtonHand == EHand.RIGHT)
            {
                if (entity.BACGeneralComp.RightClickThumbPosition == EThumbPosition.NONE &&
                    entity.BACGeneralComp.RightTouchThumbPosition == EThumbPosition.NONE)
                {
                    Debug.LogError("<b>[VRSF] :</b> You need to assign a Thumb Position for the Right Thumbstick in this script : " + entity.BACGeneralComp.name);
                    return false;
                }

                entity.BACCalculationsComp.ThumbPos = _inputsContainer.RightThumbPosition;
            }

            return true;
        }


        /// <summary>
        /// Check that the ActionButton chosed by the user is corresponding to the SDK that was loaded.
        /// </summary>
        /// <returns>true if the ActionButton is correctly set</returns>
        private bool CheckActionButton(Filter entity)
        {
            // If we are using an Oculus Touch Specific Button but the device loaded is not the Oculus
            if (entity.BACCalculationsComp.IsUsingOculusButton && VRSF_Components.DeviceLoaded == EDevice.HTC_VIVE)
            {
                Debug.LogError("The Button Action Choser parameters for the " + entity.BACCalculationsComp.transform.name + " object are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Oculus. Disabling the script.");
                return false;
            }
            // If we are using an OpenVR Specific Button but the device loaded is not the OpenVR
            else if (entity.BACCalculationsComp.IsUsingViveButton && VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT)
            {
                Debug.LogError("The Button Action Choser parameters for the " + entity.BACCalculationsComp.transform.name + " object are invalid.\n" +
                    "Please specify a button that is available for the current device (" + VRSF_Components.DeviceLoaded + ") and not only for the Vive. Disabling the script.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void StartBACsSetup(SDKChoserIsSetup info)
        {
            foreach (var entity in GetEntities<Filter>())
            {
                // We check on which hand is set the Action Button selected
                CheckButtonHand(entity);

                // We check that all the parameters are set correctly
                if (entity.BACCalculationsComp.ParametersAreInvalid || !CheckParameters(entity))
                {
                    Debug.LogError("The Button Action Choser parameters for the ButtonActionChoserComponents on the " + entity.BACGeneralComp.transform.name + " object are invalid.\n" +
                        "Please specify valid values as displayed in the Help Boxes under your script. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                    entity.BACCalculationsComp.CanBeUsed = false;
                }
                else
                {
                    entity.BACCalculationsComp.ActionButtonIsReady = true;
                }
            }
            new OnActionButtonIsReady();
        }
        #endregion PRIVATES_METHODS
    }
}