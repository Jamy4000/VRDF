using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Interactions;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;
using VRSF.Core.VRInteraction;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Contains all variable necessary for the StepByStepSystems to work.
    /// It implements the ITeleportComponent to be able to display the boundaries limits with TeleportBoundaries.
    /// </summary>
    [RequireComponent(typeof(SetupVRDestroyer), typeof(GeneralTeleportAuthoring), typeof(VRInteractionAuthoring))]
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

                /*
                 * Setting up Interactions
                 */

                // Add the corresponding input component for the selected button. If the button wasn't chose correctly, we destroy this entity and return.
                if (!InteractionSetupHelper.AddInputCaptureComponent(ref entityManager, ref entity, interactionParameters))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                // If the Hand wasn't chose correctly, we destroy this entity and return.
                if (!InteractionSetupHelper.AddButtonHand(ref entityManager, ref entity, interactionParameters.ButtonHand))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                // Add the corresponding interaction type component for the selected button. If the interaction type wasn't chose correctly, we destroy this entity and return.
                if (!InteractionSetupHelper.AddInteractionType(ref entityManager, ref entity, interactionParameters.InteractionType, interactionParameters.ButtonToUse))
                {
                    entityManager.DestroyEntity(entity);
                    return;
                }

                /*
                 * Setting up Raycasting
                 */

                switch (interactionParameters.ButtonHand)
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

                var generalTeleportParam = GetComponent<GeneralTeleportAuthoring>();

                entityManager.SetComponentData(entity, new VRRaycastParameters
                {
                    MaxRaycastDistance = _distanceStepByStep,
                    ExcludedLayer = generalTeleportParam.ExcludedLayers
                });

                entityManager.SetComponentData(entity, new VRRaycastOutputs
                {
                    RaycastHitVar = new RaycastHitVariable(),
                    RayVar = new Ray()
                });

                /*
                 * Setting up TeleportStuffs
                 */

                entityManager.SetComponentData(entity, new StepByStepComponent
                {
                    DistanceStepByStep = _distanceStepByStep,
                    StepHeight = _stepHeight
                });

                entityManager.SetComponentData(entity, new GeneralTeleportParameters
                {
                    IsUsingFadingEffect = generalTeleportParam.IsUsingFadingEffect
                });

                var tnm = GetComponent<TeleportNavMeshAuthoring>();

                entityManager.SetComponentData(entity, new TeleportNavMesh
                {
                    IgnoreSlopedSurfaces = tnm.IgnoreSlopedSurfaces,
                    NavAreaMask = tnm.NavAreaMask,
                    QueryTriggerInteraction = tnm.QueryTriggerInteraction,
                    SampleRadius = tnm.SampleRadius
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