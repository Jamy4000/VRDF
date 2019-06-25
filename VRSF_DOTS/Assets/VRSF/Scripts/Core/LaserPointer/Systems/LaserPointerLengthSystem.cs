using E7.ECS.LineRenderer;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Handle the Length of the Pointer depending on if the raycast is hitting something
    /// </summary>
    public class LaserPointerLengthSystem : JobComponentSystem
    {
        #region ComponentSystem_Methods
        // Update is called once per frame
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            return new SetLineSegmentPositionsJob().Schedule(this, inputDeps);
        }
        #endregion ComponentSystem_Methods

        [Unity.Burst.BurstCompile]
        struct SetLineSegmentPositionsJob : IJobForEach<LaserPointerLength, LineSegment, VRRaycastOrigin, VRRaycastOutputs>
        {
            public void Execute(ref LaserPointerLength laserLength, ref LineSegment lineSegment, ref VRRaycastOrigin raycastOrigin, ref VRRaycastOutputs raycastOutputs)
            {
                lineSegment.from = raycastOrigin.RayOriginPosition;

                if (raycastOutputs.RaycastHitVar.IsNull)
                {
                    UnityEngine.Debug.Log("IS NULL");
                }

                // Reduce lineRenderer from the controllers position to the object that was hit
                // OR put back lineRenderer to its normal length if nothing was hit
                lineSegment.to = raycastOutputs.RaycastHitVar.IsNull ? new float3(0.0f, 0.0f, laserLength.BaseLength) : (float3)raycastOutputs.RaycastHitVar.Value.point;
            }
        }
    }
}