using UnityEngine;
using UnityEngine.Events;
using VRSF.Core.Inputs;
using VRSF.Core.Controllers;
using Unity.Entities;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;
using System;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Let you assign a response to one of the button on the Controllers of your choice.
    /// </summary>
    [RequireComponent(typeof(SetupVRDestroyer))]
    public class ControllersButtonResponseAssigner : MonoBehaviour
    {
        [Header("The Devices that are using this CBRA Script")]
        [HideInInspector] public EDevice DeviceUsingCBRA = EDevice.ALL;


        [Header("The type of Interaction you want to use")]
        [HideInInspector] public EControllerInteractionType InteractionType = EControllerInteractionType.NONE;


        [Header("The hand on which the button to use is situated")]
        [HideInInspector] public EHand ButtonHand;


        [Header("The button you wanna use for the Action")]
        [HideInInspector] public EControllersButton ButtonToUse = EControllersButton.NONE;


        [Header("Thumbs Parameters")]
        [Tooltip("The position of the thumb you wanna use for the Touch Action")]
        [HideInInspector] public EThumbPosition TouchThumbPosition = EThumbPosition.NONE;
        [Tooltip("The position of the thumb you wanna use for the Click Action")]
        [HideInInspector] public EThumbPosition ClickThumbPosition = EThumbPosition.NONE;

        [Tooltip("At which threshold is the IsTouching event raised ? Absolute Value between 0 and 1")]
        [HideInInspector] public float IsTouchingThreshold = 0.1f;
        [Tooltip("At which threshold is the IsClicking event raised ? Absolute Value between 0 and 1")]
        [HideInInspector] public float IsClickingThreshold = 0.1f;


        [Header("The UnityEvents called when the user is Touching")]
        [HideInInspector] public UnityEvent OnButtonStartTouching;
        [HideInInspector] public UnityEvent OnButtonStopTouching;
        [HideInInspector] public UnityEvent OnButtonIsTouching;


        [Header("The UnityEvents called when the user is Clicking")]
        [HideInInspector] public UnityEvent OnButtonStartClicking;
        [HideInInspector] public UnityEvent OnButtonStopClicking;
        [HideInInspector] public UnityEvent OnButtonIsClicking;

        private void Awake()
        {
            OnSetupVRReady.Listeners += CreateEntity;
        }

        private void OnDestroy()
        {
            OnSetupVRReady.Listeners -= CreateEntity;
        }

        private void CreateEntity(OnSetupVRReady info)
        {
            // If the device loaded is included in the device using this CBRA
            if ((DeviceUsingCBRA & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.Active.EntityManager;

                var archetype = entityManager.CreateArchetype
                (
                    typeof(BaseInputCapture),
                    typeof(CBRAInteractionType)
                );

                var entity = entityManager.CreateEntity(archetype);

                // Add the corresponding input component for the selected button. If the button wasn't chose correctly, we destroy this entity and return.
                if (!CBRASetupHelper.AddButtonInputComponent(ref entityManager, ref entity, ButtonToUse, ButtonHand, TouchThumbPosition, ClickThumbPosition, IsTouchingThreshold, IsClickingThreshold))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                // If the Hand wasn't chose correctly, we destroy this entity and return.
                if (!CBRASetupHelper.AddButtonHand(ref entityManager, ref entity, ButtonHand))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                // Add the corresponding interaction type component for the selected button. If the interaction type wasn't chose correctly, we destroy this entity and return.
                if (!CBRASetupHelper.AddInteractionType(ref entityManager, ref entity, InteractionType, ButtonToUse, out CBRAInteractionType cbraInteraction))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                bool cbraHasEvents = false;

                // If at least one of the unity event for the click has a persistent listener set in the editor
                // Add the CBRA Click Events component to the ClickEvents dictionary
                if (OnButtonStartClicking.GetPersistentEventCount() > 0 || OnButtonStartClicking.GetNonPersistentListenersCount() > 0)
                {
                    cbraHasEvents = true;
                    CBRADelegatesHolder.StartClickingEvents.Add(entity, new Action(delegate { OnButtonStartClicking.Invoke(); }));
                }
                if (OnButtonIsClicking.GetPersistentEventCount() > 0 || OnButtonIsClicking.GetNonPersistentListenersCount() > 0)
                {
                    cbraHasEvents = true;
                    CBRADelegatesHolder.IsClickingEvents.Add(entity, new Action(delegate { OnButtonIsClicking.Invoke(); }));
                }
                if (OnButtonStopClicking.GetPersistentEventCount() > 0 || OnButtonStopClicking.GetNonPersistentListenersCount() > 0)
                {
                    cbraHasEvents = true;
                    CBRADelegatesHolder.StopClickingEvents.Add(entity, new Action(delegate { OnButtonStopClicking.Invoke(); }));
                }

                // If at least one of the unity event for the touch has a persistent listener set in the editor
                // Add the CBRA Click Events component to the ClickEvents dictionary
                if (OnButtonStartTouching.GetPersistentEventCount() > 0 || OnButtonStartTouching.GetNonPersistentListenersCount() > 0)
                {
                    cbraHasEvents = true;
                    CBRADelegatesHolder.StartTouchingEvents.Add(entity, new Action(delegate { OnButtonStartTouching.Invoke(); }));
                }
                if (OnButtonIsTouching.GetPersistentEventCount() > 0 || OnButtonStopTouching.GetNonPersistentListenersCount() > 0)
                {
                    cbraHasEvents = true;
                    CBRADelegatesHolder.IsTouchingEvents.Add(entity, new Action(delegate { OnButtonIsTouching.Invoke(); }));
                }
                if (OnButtonStopTouching.GetPersistentEventCount() > 0 || OnButtonIsTouching.GetNonPersistentListenersCount() > 0)
                {
                    cbraHasEvents = true;
                    CBRADelegatesHolder.StopTouchingEvents.Add(entity, new Action(delegate { OnButtonStopTouching.Invoke(); }));
                }

                // Check if at least one event response was setup
                if (!cbraHasEvents)
                {
                    Debug.LogErrorFormat("<b>[VRSF] :</b> Please give at least one response to one of the Unity Events for the CBRA on Object {0}.", transform.name);
                    entityManager.DestroyEntity(entity);
                    return;
                }

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, string.Format("CBRA Entity from GO {0}", transform.name));
#endif
            }
        }
    }
}