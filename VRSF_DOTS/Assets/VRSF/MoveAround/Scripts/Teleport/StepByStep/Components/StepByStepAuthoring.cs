using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Interactions;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.Core.VRInteraction;

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

        private void Awake()
        {
            VRInteractionAuthoring interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((interactionParameters.DeviceUsingCBRA & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                var entityManager = World.Active.EntityManager;

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
                if (!TeleporterSetupHelper.SetupInteractions(ref entityManager, ref entity, interactionParameters))
                {
                    entityManager.DestroyEntity(entity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up Raycasting
                if(!TeleporterSetupHelper.SetupRaycast(ref entityManager, ref entity, interactionParameters, _distanceStepByStep))
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
                    StepHeight = _stepHeight
                });

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(entity, "StepByStep Teleport Entity");
#endif
            }

            Destroy(gameObject);
        }
    }
}