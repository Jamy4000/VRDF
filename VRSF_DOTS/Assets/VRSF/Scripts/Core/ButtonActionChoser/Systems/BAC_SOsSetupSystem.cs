using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Events;
using VRSF.Core.Inputs;

namespace VRSF.Core.Utils.ButtonActionChoser
{
    /// <summary>
    /// Setup the Scriptable Objects and Everything they need (Listeners Container, Listeners, Responses, ...) for a button action choser
    /// </summary>
    public class BAC_SOsSetupSystem : ComponentSystem
    {
        struct Filter
        {
            public BACGeneralComponent BACGeneralComp;
            public BACCalculationsComponent BACCalculationsComp;
        }

        #region PRIVATE_VARIBALES
        private InputVariableContainer _inputsContainer;
        private delegate void OnButtonDelegate();
        List<BAC_DelegatesActions> _bacDelegatesList = new List<BAC_DelegatesActions>();
        #endregion PRIVATE_VARIABLES


        #region ComponentSystem_Methods
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        protected override void OnCreateManager()
        {
            OnActionButtonIsReady.Listeners += Init;
            _inputsContainer = InputVariableContainer.Instance;
            base.OnCreateManager();
            this.Enabled = false;
        }

        protected override void OnUpdate() { }

        protected override void OnDestroyManager()
        {
            OnActionButtonIsReady.Listeners -= Init;
            base.OnDestroyManager();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Check that the Initialization of the ScriptableObjects are done properly.
        /// </summary>
        private void CheckInitSOs(Filter entity)
        {
            // We check that the interaction type is correct
            if (entity.BACGeneralComp.InteractionType == EControllerInteractionType.NONE)
            {
                Debug.LogError("<b>[VRSF] :</b> Please specify a correct InteractionType for the " + this.GetType().Name + " script.\n" +
                    "Setting CanBeUsed of ButtonActionChoserComponents to false.");
                entity.BACCalculationsComp.CanBeUsed = false;
            }

            // We init the Scriptable Object references and how they work
            if (!SetupButton(entity))
            {
                Debug.LogError("<b>[VRSF] :</b> An error has occured while initializing the Scriptable Objects reference in the " + this.GetType().Name + " script.\n" +
                    "If the error persist after reloading the Editor, please open an issue on Github. Setting CanBeUsed of ButtonActionChoserComponents to false.");
                entity.BACCalculationsComp.CanBeUsed = false;
            }

            // We setup the listeners only one time as they're gonna check each entities containing the bac Componenent
            SetupListeners(entity);

            entity.BACCalculationsComp.IsSetup = true;
        }


        /// <summary>
        /// Setup the comp._isClicking and _isTouching BoolVariable depending on the comp.InteractionType and the comp._buttonHand variable.
        /// </summary>
        /// <returns>true if everything was setup correctly</returns>
        private bool SetupButton(Filter entity)
        {
            // If the Interaction Type contains at least CLICK
            if ((entity.BACGeneralComp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                switch (entity.BACGeneralComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        entity.BACCalculationsComp.IsClicking = _inputsContainer.RightClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(entity.BACGeneralComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        entity.BACCalculationsComp.IsClicking = _inputsContainer.LeftClickBoolean.Items[ControllerInputToSO.GetClickVariableFor(entity.BACGeneralComp.ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            // If the Interaction Type contains at least TOUCH
            if ((entity.BACGeneralComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                // Handle Touch events
                switch (entity.BACGeneralComp.ButtonHand)
                {
                    case EHand.RIGHT:
                        entity.BACCalculationsComp.IsTouching = _inputsContainer.RightTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(entity.BACGeneralComp.ActionButton)];
                        break;
                    case EHand.LEFT:
                        entity.BACCalculationsComp.IsTouching = _inputsContainer.LeftTouchBoolean.Items[ControllerInputToSO.GetTouchVariableFor(entity.BACGeneralComp.ActionButton)];
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Create and Setup the GameEventListeners for the Click and the Touch Events
        /// </summary>
        private void SetupListeners(Filter entity)
        {
            var delegatesHandler = new BAC_DelegatesActions(entity.BACGeneralComp, entity.BACCalculationsComp);

            if ((entity.BACGeneralComp.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
            {
                ButtonClickEvent.RegisterListener(delegatesHandler.StartActionDown);
                ButtonUnclickEvent.RegisterListener(delegatesHandler.StartActionUp);
            }

            if ((entity.BACGeneralComp.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
            {
                ButtonTouchEvent.RegisterListener(delegatesHandler.StartActionTouched);
                ButtonUntouchEvent.RegisterListener(delegatesHandler.StartActionUntouched);
            }

            _bacDelegatesList.Add(delegatesHandler);
        }

        private void UnregisterListeners()
        {
            foreach (var delegatesHandler in _bacDelegatesList)
            {
                if ((delegatesHandler.BACGeneral.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK)
                {
                    ButtonClickEvent.UnregisterListener(delegatesHandler.StartActionDown);
                    ButtonUnclickEvent.UnregisterListener(delegatesHandler.StartActionUp);
                }

                if ((delegatesHandler.BACGeneral.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                {
                    ButtonTouchEvent.UnregisterListener(delegatesHandler.StartActionTouched);
                    ButtonUntouchEvent.UnregisterListener(delegatesHandler.StartActionUntouched);
                }
            }

            _bacDelegatesList.Clear();
        }


        /// <summary>
        /// As some values are initialized in other Systems, we just want to be sure that everything is setup before checking the Scriptable Objects.
        /// </summary>
        private void Init(OnActionButtonIsReady info)
        {
            UnregisterListeners();

            foreach (var entity in GetEntities<Filter>())
            {
                var sdkChoser = entity.BACGeneralComp.GetComponent<SDKChoserComponent>();

                if (sdkChoser == null || (sdkChoser != null && entity.BACCalculationsComp.CorrectSDK))
                {
                    CheckInitSOs(entity);
                }
                else
                {
                    // SDK Choser is not using the correct SDK, feature can't be used
                    entity.BACCalculationsComp.CanBeUsed = false;
                    entity.BACCalculationsComp.IsSetup = true;
                }
            }
        }
        #endregion
    }

}