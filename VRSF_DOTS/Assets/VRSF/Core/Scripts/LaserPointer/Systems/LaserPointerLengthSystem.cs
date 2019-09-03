using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.SetupVR;

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
            _uiLayer = LayerMask.NameToLayer("UI");
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ref LaserPointerLength laserLength, ref VRRaycastOrigin raycastOrigin, ref VRRaycastOutputs raycastOutputs) =>
            {
                Transform originTransform = VRSF_Components.VRCamera.transform;

                // Depending on the RayOrigin, we provide references to different ray and raycastHit variables
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        originTransform = VRSF_Components.LeftController.transform;
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        originTransform = VRSF_Components.RightController.transform;
                        break;
                    case ERayOrigin.CAMERA:
                        // No need to set it, already done before
                        // originTransform = VRSF_Components.VRCamera.transform;
                        break;
                    default:
                        Debug.LogError("<b>[VRSF] :</b> An error has occured in the RayCalculationsSystems. " +
                            "Please check that the RayOrigin for your VRRaycatAuthoring Components are set correctly. Using Camera as Origin.");
                        break;
                }

                // Reduce lineRenderer from the controllers position to the object that was hit
                // OR put back lineRenderer to its normal length if nothing was hit
                Vector3 newEndPoint = raycastOutputs.RaycastHitVar.IsNull || raycastOutputs.RaycastHitVar.Value.collider == null ?
                    originTransform.InverseTransformDirection(raycastOutputs.RayVar.direction) : originTransform.InverseTransformPoint(CheckEndPoint(laserLength, raycastOutputs));

                new OnLaserLengthChanged(raycastOrigin.RayOrigin, newEndPoint);
            });
        }

        private Vector3 CheckEndPoint(LaserPointerLength laserLength, VRRaycastOutputs raycastOutputs)
        {
            return ShouldAimForCenter() ? raycastOutputs.RaycastHitVar.Value.collider.bounds.center : raycastOutputs.RaycastHitVar.Value.point;

            bool ShouldAimForCenter()
            {
                return (raycastOutputs.RaycastHitVar.Value.collider.gameObject.layer == _uiLayer && laserLength.ShouldPointToUICenter) || laserLength.ShouldPointTo3DObjectsCenter;
            }
        }
    }
}