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
            GameObject currentHoveredObject = HoveringVariablesContainer.GetCurrentHit(origin);

            //If nothing is hit and something was previously hovered, we raise the stop hovering event
            if (hitVar.IsNull && currentHoveredObject != null)
            {
                new OnStopHoveringObject(origin, currentHoveredObject);
            }
            //If something is hit
            else if (!hitVar.IsNull)
            {
                var objectHit = hitVar.Value.collider.gameObject;

                // Event raised only when a new object is hovered
                if (currentHoveredObject == null)
                {
                    new OnStartHoveringObject(origin, objectHit);
                }
                // If the user started to hover a new object
                else if (objectHit != currentHoveredObject)
                {
                    // We tell the system that we stopped hovering the current object. 
                    // The Systems will then be notify on the next frame that something new is being hovered
                    new OnStopHoveringObject(origin, currentHoveredObject);
                }
                // If we're still hovering the same object as the previous frame
                else
                {
                    new OnObjectIsBeingHovered(origin, currentHoveredObject);
                }
            }
        }
    }
}