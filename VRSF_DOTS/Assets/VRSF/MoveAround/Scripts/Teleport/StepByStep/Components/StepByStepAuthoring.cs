using Unity.Entities;
using UnityEngine;
using VRSF.Core.Inputs;
using VRSF.Core.VRInteractions;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Contains all variable necessary for the StepByStepSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(GeneralTeleportAuthoring), typeof(VRInteractionAuthoring))]
    public class StepByStepAuthoring : MonoBehaviour
    {
        [Header("Teleport Step by Step Parameters")]
        [Tooltip("The distance in Meters to move the Camera for the step by step feature.")]
        [SerializeField] private float _distanceStepByStep = 0.5f;

        [Tooltip("The step height of the NavMesh for the Teleport feature. Should be equal to the one specified in the Navigation Window.")]
        [SerializeField] private float _stepHeight = 0.5f;

        [Header("Other Parameters")]
        [Tooltip("Should we destroy this entity when the active scene is changed ?.")]
        [SerializeField] private bool _destroyOnSceneUnloaded = true;
        [Tooltip("Meant for Debug, if true, a red ray will be displayed on your scene view with the directon of the SBS Calculations, and a blue one will be shown to display the Raycast done to check for the teleportable/NavMesh surface. DONT FORGET TO TURN GIZMOS ON !")]
        [SerializeField] private bool _debugCalculationRays;

        private void Awake()
        {
            VRInteractionAuthoring interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((interactionParameters.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var archetype = entityManager.CreateArchetype
                (
                    typeof(BaseInputCapture),
                    typeof(ControllersInteractionType),
                    typeof(VRRaycastOutputs),
                    typeof(VRRaycastOrigin),
                    typeof(VRRaycastParameters),
                    typeof(StepByStepComponent),
                    typeof(GeneralTeleportParameters),
                    typeof(TeleportNavMesh)
                );

                var entity = entityManager.CreateEntity(archetype);

                // Setting up Interactions
                if (!InteractionSetupHelper.SetupInteractions(ref entityManager, ref entity, interactionParameters))
                {
                    entityManager.DestroyEntity(entity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up Raycasting
                if(!TeleporterSetupHelper.SetupRaycast(ref entityManager, ref entity, interactionParameters, 10))
                {
                    entityManager.DestroyEntity(entity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up General Teleporter Stuffs
                TeleporterSetupHelper.SetupTeleportStuffs(ref entityManager, ref entity, GetComponent<GeneralTeleportAuthoring>());

                // Setup Specific sbs teleporter
                entityManager.SetComponentData(entity, new StepByStepComponent
                {
                    DistanceStepByStep = _distanceStepByStep,
                    StepHeight = _stepHeight,
                    DebugCalculationsRay = _debugCalculationRays
                });

                if (_destroyOnSceneUnloaded)
                    entityManager.AddComponentData(entity, new DestroyOnSceneUnloaded { SceneIndex = gameObject.scene.buildIndex });

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, "StepByStep Teleport Entity");
#endif
            }

            Destroy(gameObject);
        }
    }
}