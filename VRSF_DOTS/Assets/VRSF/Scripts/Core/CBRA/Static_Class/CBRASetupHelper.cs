using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;

namespace VRSF.Core.CBRA
{
    public static class CBRASetupHelper
    {
        /// <summary>
        /// Add the corresponding Input component for the selected button. 
        /// </summary>
        public static bool AddButtonInputComponent(ref EntityManager entityManager, ref Entity entity, EControllersButton buttonToUse, EHand hand, EThumbPosition touchThumbPos, EThumbPosition clickThumbPosition, float isTouchingThreshold, float isClickingThreshold)
        {
            // Add the BaseInputCapture component to the entity
            entityManager.SetComponentData(entity, new BaseInputCapture());

            // Add the specific inputCapture component for our button
            switch (buttonToUse)
            {
                case EControllersButton.A_BUTTON:
                    entityManager.AddComponentData(entity, new AButtonInputCapture());
                    return true;
                case EControllersButton.B_BUTTON:
                    entityManager.AddComponentData(entity, new BButtonInputCapture());
                    return true;
                case EControllersButton.X_BUTTON:
                    entityManager.AddComponentData(entity, new XButtonInputCapture());
                    return true;
                case EControllersButton.Y_BUTTON:
                    entityManager.AddComponentData(entity, new YButtonInputCapture());
                    return true;
                case EControllersButton.THUMBREST:
                    entityManager.AddComponentData(entity, new ThumbrestInputCapture(hand));
                    return true;

                case EControllersButton.BACK_BUTTON:
                    entityManager.AddComponentData(entity, new GoAndGearVRInputCapture());
                    return true;

                case EControllersButton.TRIGGER:
                    entityManager.AddComponentData(entity, new TriggerInputCapture(hand));
                    return true;
                case EControllersButton.GRIP:
                    entityManager.AddComponentData(entity, new GripInputCapture(hand));
                    return true;
                case EControllersButton.MENU:
                    entityManager.AddComponentData(entity, new MenuInputCapture(hand));
                    return true;
                case EControllersButton.TOUCHPAD:
                    // We check that the thumbposition give in the inspector is not set as none
                    if (touchThumbPos != EThumbPosition.NONE)
                    {
                        entityManager.AddComponentData(entity, new TouchpadInputCapture(hand));
                        entityManager.AddComponentData(entity, new CBRAThumbPosition { TouchThumbPosition = touchThumbPos, IsTouchingThreshold = isTouchingThreshold, ClickThumbPosition = clickThumbPosition, IsClickingThreshold = isClickingThreshold });
                        return true;
                    }

                    Debug.LogErrorFormat("[b]VRSF :[\b] Please Specify valid Thumb Positions to use for your ControllersButtonResponseAssigners.");
                    return false;

                default:
                    Debug.LogErrorFormat("[b]VRSF :[\b] Please Specify valid buttons to use for your ControllersButtonResponseAssigners.");
                    return false;
            }
        }

        /// <summary>
        /// Add the corresponding hand component for the selected button. 
        /// </summary>
        public static bool AddButtonHand(ref EntityManager entityManager, ref Entity entity, EHand hand)
        {
            // If the button hand wasn't set in editor, we destroy this entity and return.
            if (hand != EHand.LEFT && hand != EHand.RIGHT)
            {
                Debug.LogErrorFormat("[b]VRSF :[\b] Please Specify valid Hands to use for your ControllersButtonResponseAssigners.");
                return false;
            }

            // Add the CBRA Hand component to the entity
            if (hand == EHand.LEFT)
                entityManager.SetComponentData(entity, new LeftHand());
            else
                entityManager.SetComponentData(entity, new RightHand());

            return true;
        }

        /// <summary>
        /// Add the corresponding InteractionType component for the selected button. 
        /// </summary>
        public static bool AddInteractionType(ref EntityManager entityManager, ref Entity entity, EControllerInteractionType interactionType, out CBRAInteractionType cbraInteraction)
        {
            // If the button hand wasn't set in editor, we destroy this entity and return.
            if (interactionType == EControllerInteractionType.NONE)
            {
                Debug.LogErrorFormat("[b]VRSF :[\b] Please Specify valid Interaction Types to use for your ControllersButtonResponseAssigners.");
                entityManager.DestroyEntity(entity);
                cbraInteraction = new CBRAInteractionType();
                return false;
            }

            cbraInteraction = new CBRAInteractionType
            {
                InteractionType = interactionType
            };

            // Add the CBRA Interaction Type component to the entity
            entityManager.SetComponentData(entity, cbraInteraction);

            return true;
        }
    }
}
