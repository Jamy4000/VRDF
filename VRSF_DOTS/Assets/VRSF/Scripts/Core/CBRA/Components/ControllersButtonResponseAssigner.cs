using UnityEngine;
using UnityEngine.Events;
using VRSF.Core.Inputs;
using VRSF.Core.Controllers;
using Unity.Entities;

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
            // Add the corresponding input component for the selected button. If the button wasn't chose correctly, we destroy this entity and return.
            if (!AddButtonInputComponent())
            {
                dstManager.DestroyEntity(entity);
                return;
            }

            // If the Hand wasn't chose correctly, we destroy this entity and return.
            if (!AddButtonHand())
            {
                dstManager.DestroyEntity(entity);
                return;
            }

            // Add the corresponding interaction type component for the selected button. If the interaction type wasn't chose correctly, we destroy this entity and return.
            if (!AddInteractionType())
            {
                dstManager.DestroyEntity(entity);
                return;
            }

            bool cbraHasEvents = false;

            // If at least one of the unity event for the click has a persistent listener set in the editor
            if (OnButtonStartClicking.GetPersistentEventCount() > 0 || OnButtonIsClicking.GetPersistentEventCount() > 0 || OnButtonStopClicking.GetPersistentEventCount() > 0)
            {
                cbraHasEvents = true;
                // Add the CBRA Click Events component to the entity
                dstManager.AddComponentData(entity, new CBRAClickEvents
                {
                    OnButtonStartClicking = OnButtonStartClicking,
                    OnButtonIsClicking = OnButtonIsClicking,
                    OnButtonStopClicking = OnButtonStopClicking
                });
            }

            // If at least one of the unity event for the touch has a persistent listener set in the editor
            if (OnButtonStartTouching.GetPersistentEventCount() > 0 || OnButtonIsTouching.GetPersistentEventCount() > 0 || OnButtonStopTouching.GetPersistentEventCount() > 0)
            {
                cbraHasEvents = true;
                // Add the CBRA Click Events component to the entity
                dstManager.AddComponentData(entity, new CBRATouchEvents
                {
                    OnButtonStartTouching = OnButtonStartTouching,
                    OnButtonIsTouching = OnButtonIsTouching,
                    OnButtonStopTouching = OnButtonStopTouching
                });
            }

            // Check if at least one event response was setup
            if (!cbraHasEvents)
            {
                Debug.LogErrorFormat("[b]VRSF :[\b] : Please give at least one response to one of the Unity Events for the CBRA on Object {0}.", transform.name);
                dstManager.DestroyEntity(entity);
                return;
            }

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            dstManager.SetName(entity, string.Format("CBRA Entity", entity.Index));
#endif


            /// <summary>
            /// Add the corresponding Input component for the selected button. 
            /// </summary>
            bool AddButtonInputComponent()
            {
                switch (ButtonToUse)
                {
                    case EControllersButton.A_BUTTON:
                        dstManager.AddComponentData(entity, new AButtonInputCapture());
                        return true;
                    case EControllersButton.B_BUTTON:
                        dstManager.AddComponentData(entity, new BButtonInputCapture());
                        return true;
                    case EControllersButton.X_BUTTON:
                        dstManager.AddComponentData(entity, new XButtonInputCapture());
                        return true;
                    case EControllersButton.Y_BUTTON:
                        dstManager.AddComponentData(entity, new YButtonInputCapture());
                        return true;
                    case EControllersButton.THUMBREST:
                        dstManager.AddComponentData(entity, new ThumbrestInputCapture(ButtonHand));
                        return true;

                    case EControllersButton.BACK_BUTTON:
                        dstManager.AddComponentData(entity, new GoAndGearVRInputCapture());
                        return true;

                    case EControllersButton.TRIGGER:
                        dstManager.AddComponentData(entity, new TriggerInputCapture(ButtonHand));
                        return true;
                    case EControllersButton.GRIP:
                        dstManager.AddComponentData(entity, new GripInputCapture(ButtonHand));
                        return true;
                    case EControllersButton.MENU:
                        dstManager.AddComponentData(entity, new MenuInputCapture(ButtonHand));
                        return true;
                    case EControllersButton.TOUCHPAD:
                        dstManager.AddComponentData(entity, new TouchpadInputCapture(ButtonHand));
                        return true;

                    default:
                        Debug.LogErrorFormat("[b]VRSF :[\b] : Please Specify a valid button to use for the CBRA on Object {0}.", transform.name);
                        return false;
                }
            }

            /// <summary>
            /// Add the corresponding hand component for the selected button. 
            /// </summary>
            bool AddButtonHand()
            {
                // If the button hand wasn't set in editor, we destroy this entity and return.
                if (ButtonHand != EHand.LEFT && ButtonHand != EHand.RIGHT)
                {
                    Debug.LogErrorFormat("[b]VRSF :[\b] : Please Specify a valid Hand to use for the CBRA on Object {0}.", transform.name);
                    return false;
                }

                // Add the CBRA Hand component to the entity
                dstManager.AddComponentData(entity, new CBRAHand
                {
                    ButtonHand = ButtonHand
                });

                return true;
            }

            /// <summary>
            /// Add the corresponding InteractionType component for the selected button. 
            /// </summary>
            bool AddInteractionType()
            {
                // If the button hand wasn't set in editor, we destroy this entity and return.
                if (InteractionType == EControllerInteractionType.NONE)
                {
                    Debug.LogErrorFormat("[b]VRSF :[\b] : Please Specify a valid Interaction Type to use for the CBRA on Object {0}.", transform.name);
                    dstManager.DestroyEntity(entity);
                    return false;
                }

                // Add the CBRA Interaction Type component to the entity
                dstManager.AddComponentData(entity, new CBRAInteractionType
                {
                    InteractionType = InteractionType
                });

                return true;
            }
        }
    }
}