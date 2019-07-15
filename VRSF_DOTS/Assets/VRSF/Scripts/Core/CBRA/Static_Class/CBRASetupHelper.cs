using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.SetupVR;

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
                    return IsTwoHandOculusDevice() && hand == EHand.RIGHT;
                case EControllersButton.B_BUTTON:
                    entityManager.AddComponentData(entity, new BButtonInputCapture());
                    return IsTwoHandOculusDevice() && hand == EHand.RIGHT;
                case EControllersButton.X_BUTTON:
                    entityManager.AddComponentData(entity, new XButtonInputCapture());
                    return IsTwoHandOculusDevice() && hand == EHand.LEFT;
                case EControllersButton.Y_BUTTON:
                    entityManager.AddComponentData(entity, new YButtonInputCapture());
                    return IsTwoHandOculusDevice() && hand == EHand.LEFT;
                case EControllersButton.THUMBREST:
                    entityManager.AddComponentData(entity, new ThumbrestInputCapture(hand));
                    return IsTwoHandOculusDevice();

                case EControllersButton.BACK_BUTTON:
                    entityManager.AddComponentData(entity, new GoAndGearVRInputCapture());
                    return IsOneHandPortableDevice();

                case EControllersButton.TRIGGER:
                    entityManager.AddComponentData(entity, new TriggerInputCapture(hand));
                    return true;
                case EControllersButton.GRIP:
                    entityManager.AddComponentData(entity, new GripInputCapture(hand));
                    return true;
                case EControllersButton.MENU:
                    if (IsTwoHandOculusDevice() && hand == EHand.RIGHT)
                    {
                        Debug.LogError("<b>[VRSF] :</b> Menu button aren't supported on the Right Hand for Two Handed Oculus Devices.");
                        return false;
                    }

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

                    Debug.LogError("<b>[VRSF] :</b> Please Specify valid Thumb Positions to use for your ControllersButtonResponseAssigners.");
                    return false;

                default:
                    Debug.LogError("<b>[VRSF] :</b> Please Specify valid buttons to use for your ControllersButtonResponseAssigners.");
                    return false;
            }
        }

        /// <summary>
        /// Add the corresponding hand component for the selected button. 
        /// </summary>
        public static bool AddButtonHand(ref EntityManager entityManager, ref Entity entity, EHand hand)
        {
            // Add the CBRA Hand component to the entity
            switch (hand)
            {
                case EHand.LEFT:
                    entityManager.AddComponentData(entity, new LeftHand());
                    return true;

                case EHand.RIGHT:
                    entityManager.AddComponentData(entity, new RightHand());
                    return true;

                // If the button hand wasn't set in editor, we destroy this entity and return.
                default:
                    Debug.LogError("<b>[VRSF] :</b> Please Specify valid Hands to use for your ControllersButtonResponseAssigners.");
                    return false;
            }
        }

        /// <summary>
        /// Add the corresponding InteractionType component for the selected button. 
        /// </summary>
        public static bool AddInteractionType(ref EntityManager entityManager, ref Entity entity, EControllerInteractionType interactionType, EControllersButton button, out CBRAInteractionType cbraInteraction)
        {
            // If the button hand wasn't set in editor, we destroy this entity and return.
            if (interactionType == EControllerInteractionType.NONE || !InteractionIsCompatibleWithButton(interactionType, button))
            {
                Debug.LogError("<b>[VRSF] :</b> Please Specify valid Interaction Types in your ControllersButtonResponseAssigners.");
                cbraInteraction = new CBRAInteractionType();
                return false;
            }
            else
            {
                cbraInteraction = new CBRAInteractionType
                {
                    InteractionType = interactionType
                };

                // set the CBRA Interaction Type component to the entity
                entityManager.SetComponentData(entity, cbraInteraction);

                return true;
            }
        }

        private static bool InteractionIsCompatibleWithButton(EControllerInteractionType interactionType, EControllersButton button)
        {
            switch (button)
            {
                case EControllersButton.THUMBREST: 
                    // Only work with Touch Feature and Two Hand Oculus devices
                    return IsTwoHandOculusDevice() && (interactionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH;

                case EControllersButton.GRIP:
                    switch (VRSF_Components.DeviceLoaded)
                    {
                        // No grip button
                        case EDevice.GEAR_VR:
                        case EDevice.OCULUS_GO:
                            return false;
                        // Touch and Click supported
                        case EDevice.OCULUS_QUEST:
                        case EDevice.OCULUS_RIFT:
                        case EDevice.OCULUS_RIFT_S:
                            return true;
                        default:
                            // If the user wanna use the touch feature on a non-Oculus or GearVR device, we display a warning
                            if ((interactionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                                Debug.LogWarning("<b>[VRSF] :</b> Grip Button has only a touch callback on non-Oculus devices.");

                            return (interactionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK;
                    }

                case EControllersButton.MENU:
                    switch(VRSF_Components.DeviceLoaded)
                    {
                        // No menu button
                        case EDevice.GEAR_VR:
                        case EDevice.OCULUS_GO:
                            return false;
                        default:
                            // If the user wanna use the touch feature on a Menu button, we display a warning
                            if ((interactionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH)
                                Debug.LogWarning("<b>[VRSF] :</b> Menu Button has only a Click Callback.");

                            return (interactionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK;
                    }

                case EControllersButton.BACK_BUTTON:
                    // Only work with Click Feature and Portable VR
                    return (interactionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK;

                default:
                    return true;
            }
        }

        private static bool IsTwoHandOculusDevice()
        {
            return VRSF_Components.DeviceLoaded == EDevice.OCULUS_QUEST || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT || VRSF_Components.DeviceLoaded == EDevice.OCULUS_RIFT_S;
        }

        private static bool IsOneHandPortableDevice()
        {
            return VRSF_Components.DeviceLoaded == EDevice.GEAR_VR || VRSF_Components.DeviceLoaded == EDevice.OCULUS_GO;
        }
    }
}
