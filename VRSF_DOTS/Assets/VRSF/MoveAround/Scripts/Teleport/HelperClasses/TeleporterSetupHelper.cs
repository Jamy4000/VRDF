using Unity.Entities;
using UnityEngine;
using VRSF.Core.Controllers;
using VRSF.Core.Raycast;
using VRSF.Core.VRInteractions;

namespace VRSF.MoveAround.Teleport
{
    public static class TeleporterSetupHelper
    {
        public static bool SetupRaycast(ref EntityManager entityManager, ref Entity entity, VRInteractionAuthoring interactionParameters, float distanceRaycast)
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
                MaxRaycastDistance = distanceRaycast,
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
            var queryCount = entityManager.CreateEntityQuery(typeof(Core.FadingEffect.CameraFadeParameters)).CalculateEntityCount();
            bool hasFadingCanvas = GameObject.FindObjectOfType<Core.FadingEffect.CameraFadeAuthoring>() != null || queryCount > 0;

            if (generalTeleportParam.IsUsingFadingEffect && !hasFadingCanvas)
                Debug.LogError("<b>[VRSF] :</b> You requested to use a fading effect for the teleport feature on GameObject " + generalTeleportParam.gameObject.name + ", but no CameraFadeAuthoring is placed under your Camera." +
                    "Try to do in your hierarchy RightClick > VRSF > Utils > Add Camera Fade.");

            entityManager.SetComponentData(entity, new GeneralTeleportParameters
            {
                IsUsingFadingEffect = generalTeleportParam.IsUsingFadingEffect && hasFadingCanvas
            });

            var tnm = generalTeleportParam.GetComponent<TeleportNavMeshAuthoring>();

            entityManager.SetComponentData(entity, new TeleportNavMesh
            {
                IgnoreSlopedSurfaces = tnm.IgnoreSlopedSurfaces,
                NavAreaMask = tnm.NavAreaMask,
                QueryTriggerInteraction = tnm.QueryTriggerInteraction,
                SampleRadius = tnm.SampleRadius,
                SphereCastRadius = tnm.SphereCastRadius
            });
        }
    }
}
