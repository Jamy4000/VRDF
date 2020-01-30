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
                Transform originTransform;

                // Depending on the RayOrigin, we provide references to different ray and raycastHit variables
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        if (!FetchTransformOrigin(VRSF_Components.LeftController, out originTransform))
                            return;
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        if (!FetchTransformOrigin(VRSF_Components.RightController, out originTransform))
                            return;
                        break;
                    case ERayOrigin.CAMERA:
                        if (!FetchTransformOrigin(VRSF_Components.VRCamera, out originTransform))
                            return;
                        break;
                    default:
                        Debug.LogError("<b>[VRSF] :</b> An error has occured in the RayCalculationsSystems. " +
                            "Please check that the RayOrigin for your VRRaycatAuthoring Components are set correctly. Using Camera as Origin.");
                        return;
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

        private bool FetchTransformOrigin(GameObject toFetchFrom, out Transform originTransform)
        {
            if (toFetchFrom == null)
            {
                Debug.Log("<b>[VRSF] :</b> The transform origin of the Laser couldn't be found. Skipping to next frame.");
                originTransform = null;
                return false;
            }
            else
            {
                originTransform = toFetchFrom.transform;
                return true;
            }
        }
    }
}