using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Raycast;
using VRSF.Core.Utils;
using VRSF.Core.VRInteraction;

namespace VRSF.MoveAround.Teleport
{
    public static class TeleporterSetupHelper
    {
        public static bool SetupInteractions(ref EntityManager entityManager, ref Entity entity, VRInteractionAuthoring interactionParameters)
        {
            // Add the corresponding input component for the selected button. If the button wasn't chose correctly, we destroy this entity and return.
            if (!InteractionSetupHelper.AddInputCaptureComponent(ref entityManager, ref entity, interactionParameters))
                return false;

            // If the Hand wasn't chose correctly, we destroy this entity and return.
            if (!InteractionSetupHelper.AddButtonHand(ref entityManager, ref entity, interactionParameters.ButtonHand))
                return false;

            // Add the corresponding interaction type component for the selected button. If the interaction type wasn't chose correctly, we destroy this entity and return.
            if (!InteractionSetupHelper.AddInteractionType(ref entityManager, ref entity, interactionParameters.InteractionType, interactionParameters.ButtonToUse))
                return false;

            return true;
        }

        public static bool SetupRaycast(ref EntityManager entityManager, ref Entity entity, VRInteractionAuthoring interactionParameters, float distanceSBS)
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

            var generalTeleportParam = interactionParameters.GetComponent<GeneralTeleportAuthoring>();

            entityManager.SetComponentData(entity, new VRRaycastParameters
            {
                MaxRaycastDistance = distanceSBS,
                ExcludedLayer = generalTeleportParam.ExcludedLayers
            });

            entityManager.SetComponentData(entity, new VRRaycastOutputs
            {
                RaycastHitVar = new RaycastHitVariable(),
                RayVar = new Ray()
            });

            return true;
        }

        public static void SetupTeleportStuffs(ref EntityManager entityManager, ref Entity entity, GeneralTeleportAuthoring generalTeleportParam)
        {
            entityManager.SetComponentData(entity, new GeneralTeleportParameters
            {
                IsUsingFadingEffect = generalTeleportParam.IsUsingFadingEffect
            });

            var tnm = generalTeleportParam.GetComponent<TeleportNavMeshAuthoring>();

            entityManager.SetComponentData(entity, new TeleportNavMesh
            {
                IgnoreSlopedSurfaces = tnm.IgnoreSlopedSurfaces,
                NavAreaMask = tnm.NavAreaMask,
                QueryTriggerInteraction = tnm.QueryTriggerInteraction,
                SampleRadius = tnm.SampleRadius
            });
        }
    }
}
