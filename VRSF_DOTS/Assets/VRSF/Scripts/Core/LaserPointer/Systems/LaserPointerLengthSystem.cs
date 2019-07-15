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
        private int _uiLayer;

        protected override void OnCreate()
        {
            base.OnCreate();
            _uiLayer = UnityEngine.LayerMask.NameToLayer("UI");
        }
        #region ComponentSystem_Methods

        protected override void OnUpdate()
        {
            Entities.ForEach((ref LaserPointerLength laserLength, ref VRRaycastOrigin raycastOrigin, ref VRRaycastOutputs raycastOutputs) =>
            {
                // Reduce lineRenderer from the controllers position to the object that was hit
                // OR put back lineRenderer to its normal length if nothing was hit
                float3 newEndPoint = raycastOutputs.RaycastHitVar.IsNull ? 
                    (float3)raycastOutputs.RayVar.direction * laserLength.BaseLength : 
                    CheckEndPoint(laserLength, raycastOutputs);

                new OnLaserLengthChanged(raycastOrigin.RayOrigin, newEndPoint);
            });
        }
        #endregion ComponentSystem_Methods

        private float3 CheckEndPoint(LaserPointerLength laserLength, VRRaycastOutputs raycastOutputs)
        {
            if (raycastOutputs.RaycastHitVar.Value.collider != null && raycastOutputs.RaycastHitVar.Value.collider.gameObject.layer == _uiLayer)
            {
                return laserLength.ShouldPointToUICenter ? (float3)raycastOutputs.RaycastHitVar.Value.collider.bounds.center : (float3)raycastOutputs.RaycastHitVar.Value.point;
            }
            else
            {
                return laserLength.ShouldPointTo3DObjectsCenter ? (float3)raycastOutputs.RaycastHitVar.Value.collider.bounds.center : (float3)raycastOutputs.RaycastHitVar.Value.point;
            }
        }
    }
}