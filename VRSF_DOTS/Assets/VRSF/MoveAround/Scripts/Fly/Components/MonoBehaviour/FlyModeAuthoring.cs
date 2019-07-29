using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.Fly
{
    /// <summary>
    /// Contains all references to the fly variables that are not in the FlyingParametersVariable already.
    /// </summary>
    [RequireComponent(typeof(VRInteractionAuthoring))]
    public class FlyModeAuthoring : MonoBehaviour
    {
        [Header("Speed Parameters")]
        [Tooltip("The General speed factor.")]
        public float FlyingSpeedFactor = 1.0f;

        [Tooltip("Is the user accelerating before going to the maximum speed ? Set to 0.0f if going directly at max speed.")]
        public float AccelerationEffect = 1.0f;

        [Tooltip("Is the user decelerating before stopping ? Set to 0.0f if going stopping on stop interacting.")]
        public float DecelerationFactor = 0.0f;

        [Header("Boundaries Parameters")]
        [Tooltip("The minimun local position at which the user can go. Set values to zero for no minimum boundaries.")]
        public Vector3 MinAvatarPosition = new Vector3(0.0f, 0.0f, 0.0f);

        [Tooltip("The maximum local position at which the user can go. Set values to zero for no minimum boundaries.")]
        public Vector3 MaxAvatarPosition = new Vector3(0.0f, 0.0f, 0.0f);

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
                    typeof(FlyAcceleration),
                    typeof(FlyDeceleration),
                    typeof(FlyBoundaries),
                    typeof(FlyDirection),
                    typeof(FlySpeed)
                );

                var flyModeEntity = entityManager.CreateEntity(archetype);

                // Setting up Interactions
                if (!Core.Utils.InteractionSetupHelper.SetupInteractions(ref entityManager, ref flyModeEntity, interactionParameters))
                {
                    entityManager.DestroyEntity(flyModeEntity);
                    Destroy(gameObject);
                    return;
                }

                // Setting up Raycasting
                if (!SetupRaycasting(ref entityManager, ref flyModeEntity, interactionParameters))
                {
                    entityManager.DestroyEntity(flyModeEntity);
                    Destroy(gameObject);
                    return;
                }

                // Setup Specific fly mode calculations stuffs
                entityManager.SetComponentData(flyModeEntity, new FlySpeed
                {
                    FlyingSpeedFactor = this.FlyingSpeedFactor,
                    CurrentFlightVelocity = 0.0f
                });

                entityManager.SetComponentData(flyModeEntity, new FlyDirection());

                entityManager.SetComponentData(flyModeEntity, new FlyAcceleration
                {
                    AccelerationEffectFactor = AccelerationEffect,
                    CurrentFlightVelocity = 0.0f,
                    TimeSinceStartFlying = 0.0f
                });

                entityManager.SetComponentData(flyModeEntity, new FlyDeceleration
                {
                    DecelerationEffectFactor = DecelerationFactor,
                    SlowDownTimer = 0.0f
                });

                entityManager.SetComponentData(flyModeEntity, new FlyBoundaries
                {
                    MaxAvatarPosition = this.MaxAvatarPosition,
                    MinAvatarPosition = this.MinAvatarPosition,
                    UseBoundaries = this.MaxAvatarPosition != Vector3.zero && this.MinAvatarPosition != Vector3.zero                
                });

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(flyModeEntity, "Fly Mode Entity");
#endif
            }

            Destroy(gameObject);
        }

        private bool SetupRaycasting(ref EntityManager entityManager, ref Entity entity, VRInteractionAuthoring interactionParameters)
        {
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
                    return false;
            }

            entityManager.SetComponentData(entity, new VRRaycastParameters
            {
                MaxRaycastDistance = 1.0f,
                ExcludedLayer = LayerMask.NameToLayer("IgnoreRaycast")
            });

            entityManager.SetComponentData(entity, new VRRaycastOutputs
            {
                RaycastHitVar = new RaycastHitVariable(),
                RayVar = new Ray()
            });

            return true;
        }
    }
}