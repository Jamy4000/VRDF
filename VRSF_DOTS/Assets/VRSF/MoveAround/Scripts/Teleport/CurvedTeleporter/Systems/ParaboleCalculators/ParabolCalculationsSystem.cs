using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using VRSF.Core.Inputs;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

namespace VRSF.MoveAround.Teleport
{
    /// <summary>
    /// A generic component that renders a border using the given polylines.  
    /// The borders are double sided and are oriented upwards (ie normals are parallel to the XZ plane)
    /// 
    /// Disclaimer : This script is based on the Flafla2 Vive-Teleporter Repository. You can check it out here :
    /// https://github.com/Flafla2/Vive-Teleporter
    /// </summary>
    public class ParabolCalculationsSystem : ComponentSystem
    {
        private JobHandle _rightHandle;
        private JobHandle _leftHandle;

        protected override void OnUpdate()
        {
            if (!VRSF_Components.SetupVRIsReady)
                return;

            NativeArray<float3> origin = new NativeArray<float3>(1, Allocator.TempJob);
            NativeArray<float3> baseVelocity = new NativeArray<float3>(1, Allocator.TempJob);

            // Schedule job for Left Hand
            Entities.WithAll<LeftHand>().ForEach((ref CurveTeleporterCalculations ctc) =>
            {

                Transform controller = VRSF_Components.LeftController.transform;

                origin[0] = controller.position;
                baseVelocity[0] = controller.TransformDirection(ctc.InitialVelocity);

                var job = new ParaboleCalculationsJob
                {
                    BaseVelocity = baseVelocity,
                    ParabolOrigin = origin
                }.Schedule(this, _leftHandle);
            });

            _leftHandle.Complete();

            origin.Dispose();
            baseVelocity.Dispose();

            // Schedule job for Right Hand
            Entities.WithAll<RightHand>().ForEach((ref CurveTeleporterCalculations ctc) =>
            {
                origin = new NativeArray<float3>(1, Allocator.TempJob);
                baseVelocity = new NativeArray<float3>(1, Allocator.TempJob);

                Transform controller = VRSF_Components.RightController.transform;

                origin[0] = controller.position;
                baseVelocity[0] = controller.TransformDirection(ctc.InitialVelocity);

                var job = new ParaboleCalculationsJob
                {
                    BaseVelocity = baseVelocity,
                    ParabolOrigin = origin
                }.Schedule(this, _rightHandle);
            });

            _rightHandle.Complete();

            origin.Dispose();
            baseVelocity.Dispose();
        }


        struct ParaboleCalculationsJob : IJobForEachWithEntity<CurveTeleporterCalculations, ParabolPointParameter, ParabolCalculations, GeneralTeleportParameters, TeleportNavMesh, VRRaycastParameters>
        {
            [DeallocateOnJobCompletion]
            public NativeArray<float3> BaseVelocity;
            [DeallocateOnJobCompletion]
            public NativeArray<float3> ParabolOrigin;

            public EntityManager _EntityManager;

            public void Execute(Entity e, int index, ref CurveTeleporterCalculations ctc, ref ParabolPointParameter ppp, ref ParabolCalculations parabolCalc, ref GeneralTeleportParameters gtp, ref TeleportNavMesh tnm, ref VRRaycastParameters raycastParam)
            {
                // TODO TRY THAT MOFO
                var pp = _EntityManager.GetSharedComponentData<ParabolPoints>(e);

                // TODO : Deactivate laser if it's still active
                //if (e.PointerObjects._ControllerPointer.enabled)
                //    ParabolicRendererHelper.ToggleHandLaser(e, false);

                if (gtp.CurrentTeleportState == ETeleportState.Selecting)
                {
                    // Calculate Parabola Points
                    parabolCalc.Velocity = ParaboleCalculationsHelper.ForceUpdateCurrentAngle(ctc, BaseVelocity[0]);
                    parabolCalc.Normal = ParaboleCalculationsHelper.ParabolaPointsCalculations(ref pp, ref ctc, ppp, ref parabolCalc, tnm, ParabolOrigin[0], raycastParam.ExcludedLayer, parabolCalc.Velocity);
                }
            }
        }
    }
}