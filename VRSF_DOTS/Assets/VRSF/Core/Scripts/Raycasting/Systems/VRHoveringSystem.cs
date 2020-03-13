using Unity.Entities;
using UnityEngine;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// If UseHoverFeature is set at true in the VRRaycastAuthoring Inspector, this system will check for RaycastHit and raise 
    /// the OnObjectIsBeingHovered 
    /// </summary>
    public class VRHoveringSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAll(typeof(VRHovering)).ForEach((ref VRRaycastOrigin raycastOrigin, ref VRRaycastOutputs raycastOutputs) =>
            {
                HandleOver(raycastOutputs.RaycastHitVar, raycastOrigin.RayOrigin);
            });
        }

        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        private void HandleOver(RaycastHitVariable hitVar, ERayOrigin origin)
        {
            GetCurrentHit(origin, out GameObject currentHit, out bool isOverSomething);

            Debug.Log("currentHit" + currentHit);
            Debug.Log("isOverSomething " + isOverSomething);

            //If nothing is hit and something was previously hovered, we raise the stop hovering event
            if (hitVar.IsNull && isOverSomething)
            {
                Debug.Log("OnStopHoveringObject " + currentHit);
                new OnStopHoveringObject(origin, currentHit);
            }
            //If something is hit
            else if (!hitVar.IsNull)
            {
                var objectHit = hitVar.Value.collider.gameObject;

                if (!isOverSomething)
                {
                    Debug.Log("OnStartHoveringObject " + objectHit);
                    // Event raised only when a new object is hovered
                    new OnStartHoveringObject(origin, objectHit, hitVar.Value.point);
                }
                // If the user started to hover a new object
                else if (objectHit != currentHit)
                {
                    Debug.Log("OnStopHoveringObject " + currentHit);
                    // We tell the system that we stopped hovering the current object. 
                    // The Systems will then be notify on the next frame that something new is being hovered
                    new OnStopHoveringObject(origin, currentHit);
                }
                // If we're still hovering the same object as the previous frame
                else
                {
                    new OnObjectIsBeingHovered(origin, currentHit, hitVar.Value.point);
                }
            }
        }

        private void GetCurrentHit(ERayOrigin origin, out GameObject currentHit, out bool isOverSomething)
        {
            switch (origin)
            {
                case ERayOrigin.RIGHT_HAND:
                    currentHit = InteractionVariableContainer.CurrentRightHit;
                    isOverSomething = InteractionVariableContainer.IsOverSomethingRight;
                    break;
                case ERayOrigin.LEFT_HAND:
                    currentHit = InteractionVariableContainer.CurrentLeftHit;
                    isOverSomething = InteractionVariableContainer.IsOverSomethingLeft;
                    break;
                case ERayOrigin.CAMERA:
                    currentHit = InteractionVariableContainer.CurrentGazeHit;
                    isOverSomething = InteractionVariableContainer.IsOverSomethingGaze;
                    break;
                default:
                    throw new System.Exception();
            }
        }
    }
}