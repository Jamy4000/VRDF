using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using VRSF.Core.FadingEffect;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolCalculationsSystem : ComponentSystem
    {
        private EntityManager _entityManager;
        private float3 _tempPointToGoTo;

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityManager = World.EntityManager;
        }

        [Unity.Burst.BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity teleportEntity, ref CurveTeleporterCalculations ctc, ref ParabolPointsParameters ppp, ref ParabolCalculations parabolCalc, ref GeneralTeleportParameters gtp, ref TeleportNavMesh tnm, ref VRRaycastParameters raycastParam) =>
            {
                if (gtp.CurrentTeleportState == ETeleportState.Selecting)
                {
                    NativeArray<Translation> pointsTranslation = new NativeArray<Translation>(ppp.PointCount, Allocator.Temp);
                    Transform controller = parabolCalc.Origin == Core.Controllers.EHand.LEFT ? VRSF_Components.LeftController.transform : VRSF_Components.RightController.transform;

                    // Calculate Parabola Points
                    parabolCalc.Velocity = ParaboleCalculationsHelper.ForceUpdateCurrentAngle(ctc, controller.TransformDirection(ctc.InitialVelocity));
                    parabolCalc.Normal = ParaboleCalculationsHelper.ParabolaPointsCalculations(ref pointsTranslation, ref ctc, ppp, tnm, controller.position, raycastParam.ExcludedLayer, parabolCalc.Velocity);

                    var index = 0;

                    Entities.WithAll<ParabolPointTag>().ForEach((Entity point, ref Translation translation) =>
                    {
                        if (_entityManager.GetSharedComponentData<ParabolPointParent>(point).TeleporterEntityIndex == teleportEntity.Index)
                        {
                            translation.Value = pointsTranslation[index].Value;
                            index++;
                        }
                    });

                    pointsTranslation.Dispose();
                }
                else if (gtp.CurrentTeleportState == ETeleportState.Teleporting && !gtp.HasTeleported)
                {
                    if (ctc.PointOnNavMesh || ctc.PointOnTeleportableLayer)
                    {
                        if (gtp.IsUsingFadingEffect)
                        {
                            OnFadingOutEndedEvent.Listeners += TeleportUser;
                            _tempPointToGoTo = ctc.PointToGoTo;
                            new StartFadingOutEvent(true);
                        }
                        else
                        {
                            VRSF_Components.SetVRCameraPosition(ctc.PointToGoTo);
                        }
                    }

                    gtp.HasTeleported = true;
                }
            });
        }

        private void TeleportUser(OnFadingOutEndedEvent info)
        {
            OnFadingOutEndedEvent.Listeners -= TeleportUser;
            VRSF_Components.SetVRCameraPosition(_tempPointToGoTo);
        }
    }
}