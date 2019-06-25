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
        #region ComponentSystem_Methods
        protected override void OnCreate()
        {
            OnSetupVRReady.Listeners += Setup;
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref PointerClick pointerClick, ref TriggerInputCapture triggerInputCapture, ref VRRaycastOutputs raycastOutputs) =>
            {
                if (!pointerClick.ClickEventWasFired && pointerClick.CanClick && triggerInputCapture.TriggerClick)
                {
                    switch (triggerInputCapture.Hand)
                    {
                        case EHand.LEFT:
                            CheckHit(raycastOutputs.RaycastHitVar, InteractionVariableContainer.HasClickSomethingLeft, ERayOrigin.LEFT_HAND);
                            break;
                        case EHand.RIGHT:
                            CheckHit(raycastOutputs.RaycastHitVar, InteractionVariableContainer.HasClickSomethingRight, ERayOrigin.RIGHT_HAND);
                            break;
                    }

                    pointerClick.ClickEventWasFired = true;
                }
                else if (pointerClick.ClickEventWasFired && !triggerInputCapture.TriggerClick)
                {
                    pointerClick.ClickEventWasFired = false;
                }
            });
        }

        protected override void OnDestroy()
        {
            OnSetupVRReady.Listeners -= Setup;
            // Just checking if the callbacks were indeed registered
            if (ButtonUnclickEvent.IsMethodAlreadyRegistered(ResetVariable))
                ButtonUnclickEvent.Listeners -= ResetVariable;

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
            if (GetEntityQuery(typeof(PointerClick)).CalculateLength() > 0 && !ButtonUnclickEvent.IsMethodAlreadyRegistered(ResetVariable))
            {
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