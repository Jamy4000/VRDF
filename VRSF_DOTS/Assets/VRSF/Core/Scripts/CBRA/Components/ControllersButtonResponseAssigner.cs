using UnityEngine;
using UnityEngine.Events;
using VRSF.Core.Inputs;
using Unity.Entities;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;
using System;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.CBRA
{
    /// <summary>
    /// Let you assign a response to one of the button on the Controllers of your choice.
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
        [Tooltip("Should we destroy this entity when the active scene is changed ?.")]
        [SerializeField] private bool _destroyOnSceneUnloaded = true;

        public virtual void Awake()
        {
            OnSetupVRReady.RegisterSetupVRResponse(CreateEntity);
        }

        public virtual void OnDestroy()
        {
            if (OnSetupVRReady.IsMethodAlreadyRegistered(CreateEntity))
                OnSetupVRReady.Listeners -= CreateEntity;
        }

        public void CreateEntity(OnSetupVRReady info)
        {
            var interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((interactionParameters.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var archetype = entityManager.CreateArchetype
                (
                    typeof(BaseInputCapture),
                    typeof(ControllersInteractionType),
                    typeof(CBRATag)
                );

                var entity = entityManager.CreateEntity(archetype);
                entityManager.AddComponentData(entity, new CBRATag());

                // Add the corresponding input, Hand and Interaction type component for the selected button. If the button wasn't chose correctly, we destroy this entity and return.
                if (!InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, interactionParameters))
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
                    Debug.LogError("<b>[VRSF] :</b> Please give at least one response to one of the Unity Events for the CBRA on the GameObject " + transform.name, gameObject);
                    entityManager.DestroyEntity(entity);
                    return;
                }

                if (_destroyOnSceneUnloaded)
                {
                    // Need to check for index in case the object is placed in a DontDestroyOnLoad scene
                    var sceneBuildIndex = gameObject.scene.buildIndex;
                    entityManager.AddComponentData(entity, new DestroyOnSceneUnloaded { SceneIndex = sceneBuildIndex != -1 ? gameObject.scene.buildIndex : UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex });
                }

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, string.Format("CBRA Entity from GO {0}", transform.name));
#endif
            }


            Destroy(gameObject);
        }
    }
}