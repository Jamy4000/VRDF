using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Interactions;
using VRSF.Core.Raycast;
using VRSF.Core.Utils;
using VRSF.Core.VRInteraction;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Contains all variable necessary for the StepByStepSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(GeneralTeleportAuthoring))]
    public class StepByStepAuthoring : MonoBehaviour
    {
        [Header("Teleport Step by Step Parameters")]
        [Tooltip("The distance in Meters to move the Camera for the step by step feature.")]
        private float _distanceStepByStep = 1.0f;

        [Tooltip("The step height of the NavMesh for the Teleport feature.")]
        private float _stepHeight = 1.0f;

        [Header("Layer(s) to exclude from the Raycast System.")]
        public LayerMask ExcludedLayer = new LayerMask();

        [Header("Interaction Parameters")]
        [SerializeField] private VRInteractionSet _interactionParameters;

        private void Awake()
        {
            var entityManager = World.Active.EntityManager;

            var archetype = entityManager.CreateArchetype
            (
                typeof(BaseInputCapture), 
                typeof(StepByStepComponent),
                typeof(GeneralTeleportParameters),
                typeof(VRRaycastOutputs),
                typeof(VRRaycastOrigin),
                typeof(VRRaycastParameters),
                typeof(ControllersInteractionType)
            );

            var entity = entityManager.CreateEntity(archetype);

            /*
             * Setting up Interactions
             */

            // Add the corresponding input component for the selected button. If the button wasn't chose correctly, we destroy this entity and return.
            if (!InteractionSetupHelper.AddButtonInputComponent(ref entityManager, ref entity, _interactionParameters))
            {
                entityManager.DestroyEntity(entity);
                return;
            }

            // If the Hand wasn't chose correctly, we destroy this entity and return.
            if (!InteractionSetupHelper.AddButtonHand(ref entityManager, ref entity, _interactionParameters.ButtonHand))
            {
                entityManager.DestroyEntity(entity);
                return;
            }

            // Add the corresponding interaction type component for the selected button. If the interaction type wasn't chose correctly, we destroy this entity and return.
            if (!InteractionSetupHelper.AddInteractionType(ref entityManager, ref entity, _interactionParameters.InteractionType, _interactionParameters.ButtonToUse))
            {
                entityManager.DestroyEntity(entity);
                return;
            }

            /*
             * Setting up Raycasting
             */

            switch (_interactionParameters.ButtonHand)
            {
                case EHand.LEFT:
                    entityManager.SetComponentData(entity, new VRRaycastOrigin { RayOrigin = ERayOrigin.LEFT_HAND });
                    break;
                case EHand.RIGHT:
                    entityManager.SetComponentData(entity, new VRRaycastOrigin { RayOrigin = ERayOrigin.RIGHT_HAND });
                    break;
                default:
                    Debug.LogError("<b>[VRSF] :</b> Please specify a valid hand on your UserRotationAuthoring Components.");
                    entityManager.DestroyEntity(entity);
                    Destroy(gameObject);
                    return;
            }

            entityManager.SetComponentData(entity, new VRRaycastParameters
            {
                MaxRaycastDistance = _distanceStepByStep,
                ExcludedLayer = ExcludedLayer
            });

            entityManager.AddComponentData(entity, new VRRaycastOutputs
            {
                RaycastHitVar = new RaycastHitVariable(),
                RayVar = new Ray()
            });

            /*
             * Setting up TeleportStuffs
             */

            entityManager.AddComponentData(entity, new LinearUserRotation
            {
                CurrentRotationSpeed = 0.0f,
                MaxRotationSpeed = _maxRotationSpeed,
                AccelerationFactor = _accelerationFactor
            });

            if (UseDecelerationEffect)
            {
                entityManager.AddComponentData(entity, new LinearRotationDeceleration
                {
                    DecelerationFactor = DecelerationFactor
                });
            }

#if UNITY_EDITOR
            // Set it's name in Editor Mode for the Entity Debugger Window
            entityManager.SetName(entity, "User Linear Rotation Entity");
#endif

            Destroy(gameObject);
        }
    }
}