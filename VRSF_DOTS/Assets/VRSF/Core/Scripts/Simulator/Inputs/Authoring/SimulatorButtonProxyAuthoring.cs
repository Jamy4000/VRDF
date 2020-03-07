using UnityEngine;
using Unity.Entities;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.Inputs
{
    /// <summary>
    /// Simulate the click of a button on a controller, but using the Simulator
    /// </summary>
    [RequireComponent(typeof(VRInteractionAuthoring))]
    public class SimulatorButtonProxyAuthoring : MonoBehaviour
    {
        [Header("The button used to simulate a controller's button")]
        [Tooltip("Simply set this KeyCode and the VRInteractionAuthoring next to this component to be able to simulate the controller's button with the simulator")]
        [SerializeField] private KeyCode _simulationKeyCode;

        [Tooltip("Should we destroy this entity when the active scene is changed ?")]
        [SerializeField] private bool _destroyEntityOnSceneUnloaded = true;

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetComponent<VRInteractionAuthoring>().DeviceUsingFeature = SetupVR.EDevice.SIMULATOR;
        }
#endif

        public void Awake()
        {
            OnSetupVRReady.RegisterSetupVRCallback(CreateEntity);
        }

        public void CreateEntity(OnSetupVRReady _)
        {
            OnSetupVRReady.UnregisterSetupVRCallback(CreateEntity);

            var interactionParameters = GetComponent<VRInteractionAuthoring>();

            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var entity = entityManager.CreateEntity
            (
                typeof(BaseInputCapture),
                typeof(ControllersInteractionType),
                typeof(SimulatorButtonProxy)
            );

            entityManager.SetComponentData(entity, new SimulatorButtonProxy 
            {
                SimulatedButton = interactionParameters.ButtonToUse,
                SimulationKeyCode = _simulationKeyCode
            });

            // Add the corresponding input, Hand and Interaction type component for the selected button. 
            // If the button wasn't chose correctly or any parameter was wrongly set, we destroy this entity and return.
            if (!InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, interactionParameters))
            {
                entityManager.DestroyEntity(entity);
                return;
            }

            if (_destroyEntityOnSceneUnloaded)
                OnSceneUnloadedEntityDestroyer.CheckDestroyOnSceneUnload(entityManager, entity, gameObject.scene.buildIndex, "SimulatorButtonProxyAuthoring");

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, string.Format("SimulatorButtonProxy Entity from GO {0}", transform.name));
#endif

            Destroy(gameObject);
        }
    }
}