using Unity.Entities;
using Unity.Mathematics;
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
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        HandleOver(ref InteractionVariableContainer.CurrentLeftHit, ref InteractionVariableContainer.CurrentLeftHitPosition, ref InteractionVariableContainer.IsOverSomethingLeft, ref raycastOutputs.RaycastHitVar, ERayOrigin.LEFT_HAND);
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        HandleOver(ref InteractionVariableContainer.CurrentRightHit, ref InteractionVariableContainer.CurrentRightHitPosition, ref InteractionVariableContainer.IsOverSomethingRight, ref raycastOutputs.RaycastHitVar, ERayOrigin.RIGHT_HAND);
                        break;
                    case ERayOrigin.CAMERA:
                        HandleOver(ref InteractionVariableContainer.CurrentGazeHit, ref InteractionVariableContainer.CurrentGazeHitPosition, ref InteractionVariableContainer.IsOverSomethingGaze, ref raycastOutputs.RaycastHitVar, ERayOrigin.CAMERA);
                        break;
                }
            });
        }

        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        private void HandleOver(ref GameObject currentHit, ref float3 hitPos, ref bool isOverSomething, ref RaycastHitVariable hitVar, ERayOrigin origin)
        {
            //If nothing is hit, we set the isOver value to false
            if (hitVar.IsNull && isOverSomething)
            {
                currentHit = null;
                hitPos = float3.zero;
                isOverSomething = false;
                // Event raise with null as ObjectHovered parameter, to notify the listener that nothing is being hovered right now
                new OnObjectIsBeingHovered(origin, null);
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (!hitVar.IsNull && hitVar.Value.collider != null)
            {
                hitPos = hitVar.Value.point;
                var objectHit = hitVar.Value.collider.gameObject;

                if (objectHit != currentHit)
                {
                    isOverSomething = true;
                    currentHit = objectHit;
                    // Event raised only when a new object is hovered, and not every frame
                    new OnObjectIsBeingHovered(origin, objectHit);
                }
            }
        }
    }
}