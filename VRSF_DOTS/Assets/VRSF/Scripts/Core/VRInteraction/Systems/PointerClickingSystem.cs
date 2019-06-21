using Unity.Entities;
using VRSF.Core.Events;
using VRSF.Core.Controllers;
using VRSF.Interactions;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.Core.Interactions
{
    /// <summary>
    /// Handle the Click and Unclick event in VR. Basically link the Raycast system and the Input System.
    /// CANNOT BE JOBIFIED as we need to send transform info in the ObjectWasClickedEvent
    /// </summary>
    public class PointerClickingSystem : ComponentSystem
    {
        private EHand _handInteracting = EHand.NONE;

        #region ComponentSystem_Methods
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += Setup;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref PointerClick pointerClick) =>
            {
                if (_handInteracting == pointerClick.HandClicking)
                {
                    switch (_handInteracting)
                    {
                        case EHand.LEFT:
                            if (PointerClick.LeftTriggerCanClick)
                                CheckHit(LeftControllerRaycastData.RaycastHitVar, InteractionVariableContainer.HasClickSomethingLeft, ERayOrigin.LEFT_HAND);
                            break;
                        case EHand.RIGHT:
                            if (PointerClick.RightTriggerCanClick)
                                CheckHit(RightControllerRaycastData.RaycastHitVar, InteractionVariableContainer.HasClickSomethingRight, ERayOrigin.RIGHT_HAND);
                            break;
                    }

                    // Reset the variable
                    _handInteracting = EHand.NONE;
                }
            });
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= Setup;
            // Just checking if the callbacks were indeed registered
            if (ButtonClickEvent.IsMethodAlreadyRegistered(CheckObjectClicked))
            {
                ButtonClickEvent.Listeners -= CheckObjectClicked;
                ButtonUnclickEvent.Listeners -= ResetVariable;
            }
            base.OnDestroy();
        }
        #endregion


        #region PRIVATE_METHODS
        void CheckHit(RaycastHitVariable hitVar, bool hasClickSomething, ERayOrigin origin)
        {
            //If nothing is hit, we set the hasClickSomething value to false
            if (hitVar.IsNull)
            {
                hasClickSomething = false;
            }
            else
            {
                hasClickSomething = true;
                new ObjectWasClickedEvent(origin, hitVar.Value.collider.transform);
            }
        }

        /// <summary>
        /// Reset the HasClickSomethingRight bool if the user is not clicking anymore
        /// </summary>
        void CheckObjectClicked(ButtonClickEvent info)
        {
            if (info.ButtonInteracting == EControllersButton.TRIGGER)
                _handInteracting = info.HandInteracting;
        }

        void ResetVariable(ButtonUnclickEvent info)
        {
            if (info.ButtonInteracting == EControllersButton.TRIGGER)
            {
                if (info.HandInteracting == EHand.RIGHT)
                    InteractionVariableContainer.HasClickSomethingRight = false;
                else
                    InteractionVariableContainer.HasClickSomethingLeft = false;
            }
        }

        private void Setup(OnSetupVRReady info)
        {
            // Just checking if there's entities in the scene
            if (GetEntityQuery(typeof(PointerClick)).CalculateLength() > 0 && !ButtonClickEvent.IsMethodAlreadyRegistered(CheckObjectClicked))
            {
                ButtonClickEvent.Listeners += CheckObjectClicked;
                ButtonUnclickEvent.Listeners += ResetVariable;
            }
            else
            {
                this.Enabled = false;
            }
        }
        #endregion PRIVATE_METHODS
    }
}