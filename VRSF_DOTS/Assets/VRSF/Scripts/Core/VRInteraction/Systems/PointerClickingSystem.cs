using Unity.Entities;
using VRSF.Core.Events;
using VRSF.Core.Controllers;
using VRSF.Interactions;
using VRSF.Core.Inputs;
using ScriptableFramework.Variables;

namespace VRSF.Core.Interactions
{
    public class PointerClickingSystem : ComponentSystem
    {
        struct Filter
        {
            public PointerClickComponent PointerClick;
            public Raycast.ScriptableRaycastComponent PointerRaycast;
            // TODO : Add a new component, ControllerInputsComponent, instead on the _interactionVariables
        }

        private InteractionVariableContainer _interactionsVariables;

        #region ComponentSystem_Methods
        protected override void OnCreateManager()
        {
            base.OnCreateManager();
            _interactionsVariables = InteractionVariableContainer.Instance;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            // Just checking if there's entities in the scene
            foreach (var e in GetEntities<Filter>())
            {
                ButtonClickEvent.Listeners += CheckObjectClicked;
                ButtonUnclickEvent.Listeners += ResetVariable;
                return;
            }
        }

        protected override void OnUpdate() {}

        protected override void OnDestroyManager()
        {
            // Just checking if there's entities in the scene
            foreach (var e in GetEntities<Filter>())
            {
                ButtonClickEvent.Listeners -= CheckObjectClicked;
                ButtonUnclickEvent.Listeners -= ResetVariable;
                return;
            }
            base.OnDestroyManager();
        }
        #endregion


        #region PRIVATE_METHODS
        /// <summary>
        /// Reset the HasClickSomethingRight bool if the user is not clicking anymore
        /// </summary>
        void CheckObjectClicked(ButtonClickEvent info)
        {
            if (info.ButtonInteracting == EControllersButton.TRIGGER)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (info.HandInteracting == e.PointerClick.HandClicking)
                    {
                        switch (info.HandInteracting)
                        {
                            case EHand.LEFT:
                                if (PointerClickComponent.LeftTriggerCanClick)
                                    CheckHit(_interactionsVariables.LeftHit, _interactionsVariables.HasClickSomethingLeft, Raycast.ERayOrigin.LEFT_HAND);
                                break;
                            case EHand.RIGHT:
                                if (PointerClickComponent.RightTriggerCanClick)
                                    CheckHit(_interactionsVariables.RightHit, _interactionsVariables.HasClickSomethingRight, Raycast.ERayOrigin.RIGHT_HAND);
                                    break;
                        }
                    }
                }
            }


            void CheckHit(RaycastHitVariable hitVar, BoolVariable hasClickSomething, Raycast.ERayOrigin origin)
            {
                //If nothing is hit, we set the hasClickSomething value to false
                if (hitVar.IsNull)
                {
                    hasClickSomething.SetValue(false);
                }
                else
                {
                    hasClickSomething.SetValue(true);
                    new ObjectWasClickedEvent(origin, hitVar.Value.collider.transform);
                }
            }
        }

        void ResetVariable(ButtonUnclickEvent info)
        {
            if (info.ButtonInteracting == EControllersButton.TRIGGER)
            {
                foreach (var e in GetEntities<Filter>())
                {
                    if (info.HandInteracting == EHand.RIGHT)
                        _interactionsVariables.HasClickSomethingRight.SetValue(false);
                    else
                        _interactionsVariables.HasClickSomethingLeft.SetValue(false);
                }
            }
        }
        #endregion PRIVATE_METHODS
    }
}