using System;
using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;
using VRSF.Core.Utils;
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

        private void Awake()
        {
            VRInteractionAuthoring interactionParameters = GetComponent<VRInteractionAuthoring>();

            // If the device loaded is included in the device using this CBRA
            if ((interactionParameters.DeviceUsingFeature & VRSF_Components.DeviceLoaded) == VRSF_Components.DeviceLoaded)
            {
                if (interactionParameters.ButtonToUse != EControllersButton.TOUCHPAD)
                {
                    Debug.LogError("<b>[VRSF] :</b> The Fly mode can only be use using the touchpad/thumbstick, as we use the up and down value. Entity won't be created.");
                    Destroy(gameObject);
                    return;
                }
                else if (!InteractionIsUpAndDown(interactionParameters))
                {
                    Debug.LogError("<b>[VRSF] :</b> The Fly mode can only be use using an UP/DOWN combination, as we use them to calculate the direction. Entity won't be created.");
                    Destroy(gameObject);
                    return;
                }

                var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

                var archetype = entityManager.CreateArchetype
                (
                    typeof(BaseInputCapture),
                    typeof(ControllersInteractionType),
                    typeof(VRRaycastOutputs),
                    typeof(VRRaycastOrigin),
                    typeof(VRRaycastParameters),
                    typeof(FlyAcceleration),
                    typeof(FlyDeceleration),
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

                // Check for Fly Boundaries
                var flyBoundaries = GetComponent<FlyBoundariesAuthoring>();
                if (flyBoundaries != null)
                {
                    entityManager.AddComponentData(flyModeEntity, new FlyBoundaries
                    {
                        MaxAvatarPosition = flyBoundaries.MaxAvatarPosition,
                        MinAvatarPosition = flyBoundaries.MinAvatarPosition,
                        UseBoundaries = flyBoundaries.MaxAvatarPosition != Vector3.zero && flyBoundaries.MinAvatarPosition != Vector3.zero
                    });
                }

                entityManager.AddComponentData(flyModeEntity, new DestroyOnSceneUnloaded());

#if UNITY_EDITOR
                // Set it's name in Editor Mode for the Entity Debugger Window
                entityManager.SetName(flyModeEntity, "Fly Mode Entity");
#endif
            }

            Destroy(gameObject);
        }

        /// <summary>
        /// Check if the interaction are click + UP and DOWN 
        /// OR
        /// if the interaction are touch + UP and DOWN 
        /// </summary>
        /// <param name="interactionParameters">the interaction parameters to check</param>
        /// <returns>true if at least one of the consition is respected</returns>
        private bool InteractionIsUpAndDown(VRInteractionAuthoring interactionParameters)
        {
            return ((interactionParameters.InteractionType & EControllerInteractionType.CLICK) == EControllerInteractionType.CLICK
                    && ((interactionParameters.ClickThumbPosition & EThumbPosition.DOWN) == EThumbPosition.DOWN && (interactionParameters.ClickThumbPosition & EThumbPosition.UP) == EThumbPosition.UP))
                || ((interactionParameters.InteractionType & EControllerInteractionType.TOUCH) == EControllerInteractionType.TOUCH
                    && ((interactionParameters.TouchThumbPosition & EThumbPosition.DOWN) == EThumbPosition.DOWN && (interactionParameters.TouchThumbPosition & EThumbPosition.UP) == EThumbPosition.UP));
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