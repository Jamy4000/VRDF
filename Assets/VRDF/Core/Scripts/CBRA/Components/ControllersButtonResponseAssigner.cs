﻿using UnityEngine;
using UnityEngine.Events;
using VRDF.Core.Inputs;
using Unity.Entities;
using System;
using VRDF.Core.VRInteractions;

namespace VRDF.Core.CBRA
{
    /// <summary>
    /// Let you assign a response to one of the button on the Controllers of your choice.
    /// UnityEvent are displayed from the Editor script
    /// </summary>
    [RequireComponent(typeof(VRInteractionAuthoring))]
    public class ControllersButtonResponseAssigner : MonoBehaviour
    {
        [Header("The UnityEvents called when the user is Touching")]
        [HideInInspector] public UnityEvent OnButtonStartTouching;
        [HideInInspector] public UnityEvent OnButtonStopTouching;
        [HideInInspector] public UnityEvent OnButtonIsTouching;


        [Header("The UnityEvents called when the user is Clicking")]
        [HideInInspector] public UnityEvent OnButtonStartClicking;
        [HideInInspector] public UnityEvent OnButtonStopClicking;
        [HideInInspector] public UnityEvent OnButtonIsClicking;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy the created entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

        public virtual void Awake()
        {
            OnSetupVRReady.RegisterSetupVRCallback(CreateEntity);
        }

        public void CreateEntity(OnSetupVRReady _)
        {
            OnSetupVRReady.UnregisterSetupVRCallback(CreateEntity);

            var interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device list using this CBRA
            if ((interactionParameters.DeviceUsingFeature & VRDF_Components.DeviceLoaded) == VRDF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
                var entity = entityManager.CreateEntity
                (
                    typeof(BaseInputCapture),
                    typeof(ControllersInteractionType),
                    typeof(CBRATag)
                );

                // Add the corresponding input, Hand and Interaction type component for the selected button. 
                // If the button wasn't chose correctly or any parameter was wrongly set, we destroy this entity and return.
                if (!InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, interactionParameters))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                // help us check if this CBRA has at least one event. if false, this entity will be destroy.
                bool cbraHasEvents = false;

                if (interactionParameters.InteractionType.HasFlag(EControllerInteractionType.CLICK))
                {
                    // If at least one of the unity event for the click has a persistent listener set in the editor
                    // Add the CBRA Click Events component to the ClickEvents dictionary
                    if (EventHasACallback(OnButtonStartClicking))
                    {
                        cbraHasEvents = true;
                        CBRADelegatesHolder.StartClickingEvents.Add(entity, new Action(delegate { OnButtonStartClicking.Invoke(); }));
                    }
                    if (EventHasACallback(OnButtonIsClicking))
                    {
                        cbraHasEvents = true;
                        CBRADelegatesHolder.IsClickingEvents.Add(entity, new Action(delegate { OnButtonIsClicking.Invoke(); }));
                    }
                    if (EventHasACallback(OnButtonStopClicking))
                    {
                        cbraHasEvents = true;
                        CBRADelegatesHolder.StopClickingEvents.Add(entity, new Action(delegate { OnButtonStopClicking.Invoke(); }));
                    }
                }

                if (interactionParameters.InteractionType.HasFlag(EControllerInteractionType.TOUCH))
                {
                    // If at least one of the unity event for the touch has a persistent listener set in the editor
                    // Add the CBRA Click Events component to the ClickEvents dictionary
                    if (EventHasACallback(OnButtonStartTouching))
                    {
                        cbraHasEvents = true;
                        CBRADelegatesHolder.StartTouchingEvents.Add(entity, new Action(delegate { OnButtonStartTouching.Invoke(); }));
                    }
                    if (EventHasACallback(OnButtonIsTouching))
                    {
                        cbraHasEvents = true;
                        CBRADelegatesHolder.IsTouchingEvents.Add(entity, new Action(delegate { OnButtonIsTouching.Invoke(); }));
                    }
                    if (EventHasACallback(OnButtonStopTouching))
                    {
                        cbraHasEvents = true;
                        CBRADelegatesHolder.StopTouchingEvents.Add(entity, new Action(delegate { OnButtonStopTouching.Invoke(); }));
                    }
                }

                // Check if at least one event response was setup
                if (!cbraHasEvents)
                {
                    Debug.LogError("<Color=red><b>[VRDF] :</b> Please give at least one response to one of the Unity Events for the CBRA on the GameObject.</Color>" + transform.name, gameObject);
                    entityManager.DestroyEntity(entity);
                    return;
                }

                if (_destroyEntityOnSceneUnloaded)
                    OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(ref entityManager, ref entity, gameObject.scene.buildIndex, "CBRA");

                // If we use the simulator, we check for a SimulatorButtonProxy. if not null, we add the simulatorButtonProxy script
                if (VRDF_Components.DeviceLoaded == SetupVR.EDevice.SIMULATOR)
                    GetComponent<Simulator.SimulatorButtonProxyAuthoring>()?.AddSimulatorButtonProxy(ref entityManager, ref entity, interactionParameters);

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, string.Format("CBRA Entity from GO {0}", transform.name));
#endif
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Check if the UnityEvent pass as parameter has persisten or non persistent listeners
        /// </summary>
        /// <param name="toCheck">The event that we want to inspect</param>
        /// <returns>true if at least one event was registered in the Persisten or the NonPersistent listeners</returns>
        private bool EventHasACallback(UnityEvent toCheck)
        {
            return toCheck.GetPersistentEventCount() > 0 || toCheck.GetNonPersistentListenersCount() > 0;
        }
    }
}