using Unity.Entities;
using UnityEngine;
using VRSF.Core.Raycast;

namespace VRSF.Core.LaserPointer
{
    /// <summary>
    /// Handle the Length of the Pointer depending on if the raycast is hitting something
    /// </summary>
    public class LaserPointerLengthSystem : ComponentSystem
    {
        /// <summary>
        /// The byte value of the UI Layer
        /// </summary>
        private int _uiLayer;

        protected override void OnCreate()
        {
            base.OnCreate();
            _uiLayer = LayerMask.NameToLayer("UI");
        }

        [Unity.Burst.BurstCompile]
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

                // Reduce lineRenderer from the controllers position to the object that was hit OR put back lineRenderer to its normal length if nothing was hit
                var hitVar = raycastOutputs.RaycastHitVar;
                Vector3 newEndPoint = hitVar.IsNull || hitVar.Value.collider == null ?
                    originTransform.InverseTransformDirection(raycastOutputs.RayVar.direction) : originTransform.InverseTransformPoint(CheckEndPoint(laserLength, hitVar.Value));

                // Raise the event with the new endPoint of the laser
                new OnLaserLengthChanged(raycastOrigin.RayOrigin, newEndPoint);
            });
        }

        private bool FetchTransformOrigin(GameObject toFetchFrom, out Transform originTransform)
        {
            if (toFetchFrom == null)
            {
                Debug.LogError("<b>[VRSF] :</b> The transform origin of the Laser couldn't be found. Skipping to next frame.");
                originTransform = null;
                return false;
            }
            else
            {
                originTransform = toFetchFrom.transform;
                return true;
            }
        }

        private Vector3 CheckEndPoint(LaserPointerLength laserLength, RaycastHit hitVar)
        {
            return ShouldAimForCenter() ? hitVar.collider.bounds.center : hitVar.point;

            bool ShouldAimForCenter()
            {
                return (hitVar.collider.gameObject.layer == _uiLayer && laserLength.ShouldPointToUICenter) || laserLength.ShouldPointTo3DObjectsCenter;
            }
        }
    }
}