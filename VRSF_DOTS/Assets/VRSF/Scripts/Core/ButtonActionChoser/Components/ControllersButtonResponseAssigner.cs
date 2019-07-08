using UnityEngine;
using UnityEngine.Events;
using VRSF.Core.Inputs;
using VRSF.Core.Controllers;
using Unity.Entities;
using System;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Let you assign a response to one of the button on the Controllers of your choice.
    /// </summary>
    public class ControllersButtonResponseAssigner : MonoBehaviour, IConvertGameObjectToEntity
    {
        [Header("The type of Interaction you want to use")]
        [HideInInspector] public EControllerInteractionType InteractionType = EControllerInteractionType.NONE;

        [Header("The hand on which the button to use is situated")]
        [HideInInspector] public EHand ButtonHand;

        [Header("The button you wanna use for the Action")]
        [HideInInspector] public EControllersButton ButtonToUse = EControllersButton.NONE;

        [Header("The position of the Thumb to start this feature")]
        [HideInInspector] public EThumbPosition TouchThumbPosition = EThumbPosition.NONE;
        [HideInInspector] public EThumbPosition ClickThumbPosition = EThumbPosition.NONE;

        [Header("The UnityEvents called when the user is Touching")]
        [HideInInspector] public UnityEvent OnButtonStartTouching;
        [HideInInspector] public UnityEvent OnButtonStopTouching;
        [HideInInspector] public UnityEvent OnButtonIsTouching;

        [Header("The UnityEvents called when the user is Clicking")]
        [HideInInspector] public UnityEvent OnButtonStartClicking;
        [HideInInspector] public UnityEvent OnButtonStopClicking;
        [HideInInspector] public UnityEvent OnButtonIsClicking;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var archetype = dstManager.CreateArchetype
            (
                GetButtonComponent(),
                typeof(CBRA)
            );
        }

        private Type GetButtonComponent()
        {
            switch (ButtonToUse)
            {
                case EControllersButton.A_BUTTON:
                    return typeof(AButtonInputCapture);
                case EControllersButton.B_BUTTON:
                    return typeof(BButtonInputCapture);
                case EControllersButton.X_BUTTON:
                    return typeof(XButtonInputCapture);
                case EControllersButton.Y_BUTTON:
                    return typeof(YButtonInputCapture);
                case EControllersButton.THUMBREST:
                    return typeof(ThumbrestInputCapture);

                case EControllersButton.BACK_BUTTON:
                    return typeof(GoAndGearVRInputCapture);

                case EControllersButton.TRIGGER:
                    return typeof(TriggerInputCapture);
                case EControllersButton.GRIP:
                    return typeof(GripInputCapture);
                case EControllersButton.MENU:
                    return typeof(MenuInputCapture);
                case EControllersButton.TOUCHPAD:
                    return typeof(TouchpadInputCapture);

                default:
                    Debug.LogError("[b]VRSF :[\b] : Please Specify a valid button to use.");
                    return null;
            }
        }
    }

    /// <summary>
    /// Tag for the ControllerButtonResponseAssigner component
    /// </summary>
    public struct CBRA : IComponentData { }
}