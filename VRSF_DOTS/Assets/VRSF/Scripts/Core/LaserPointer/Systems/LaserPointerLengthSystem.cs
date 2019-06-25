using Unity.Entities;
using Unity.Mathematics;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Handle the Length of the Pointer depending on if the raycast is hitting something
    /// </summary>
    public class LaserPointerLengthSystem : ComponentSystem
    {
        #region ComponentSystem_Methods
        [Unity.Burst.BurstCompile]
        protected override void OnUpdate()
        {
            Entities.ForEach((ref LaserPointerLength laserLength, ref VRRaycastOrigin raycastOrigin, ref VRRaycastOutputs raycastOutputs) =>
            {
                // Reduce lineRenderer from the controllers position to the object that was hit
                // OR put back lineRenderer to its normal length if nothing was hit
                var newEndPoint = raycastOutputs.RaycastHitVar.IsNull ? new float3(0.0f, 0.0f, laserLength.BaseLength) : (float3)raycastOutputs.RaycastHitVar.Value.point;
                new OnLaserLengthChanged(raycastOrigin.RayOrigin, newEndPoint);
            });
        }
        #endregion ComponentSystem_Methods
    }
}