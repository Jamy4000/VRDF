using Unity.Entities;
using UnityEngine;
using VRSF.Core.Events;
using VRSF.Core.Raycast;

namespace VRSF.Core.VRInteractions
{
    public class PointerHoveringSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref VRRaycastOrigin raycastOrigin, ref VRRaycastOutputs raycastOutputs) =>
            {
                switch (raycastOrigin.RayOrigin)
                {
                    case ERayOrigin.LEFT_HAND:
                        HandleOver(ref InteractionVariableContainer.PreviousLeftHit, ref InteractionVariableContainer.IsOverSomethingLeft, ref raycastOutputs.RaycastHitVar, ERayOrigin.LEFT_HAND);
                        break;
                    case ERayOrigin.RIGHT_HAND:
                        HandleOver(ref InteractionVariableContainer.PreviousRightHit, ref InteractionVariableContainer.IsOverSomethingRight, ref raycastOutputs.RaycastHitVar, ERayOrigin.RIGHT_HAND);
                        break;
                    case ERayOrigin.CAMERA:
                        HandleOver(ref InteractionVariableContainer.PreviousGazeHit, ref InteractionVariableContainer.IsOverSomethingGaze, ref raycastOutputs.RaycastHitVar, ERayOrigin.CAMERA);
                        break;
                }
            });
        }

        /// <summary>
        /// Handle the raycastHits to check if one of them touch something
        /// </summary>
        private void HandleOver(ref Transform previousHit, ref bool isOverSomething, ref RaycastHitVariable hitVar, ERayOrigin origin)
        {
            //If nothing is hit, we set the isOver value to false
            if (hitVar.IsNull && isOverSomething)
            {
                previousHit = null;
                isOverSomething = false;
                new ObjectWasHoveredEvent(origin, null);
            }
            //If something is hit, we check that the collider is still "alive", and we check that the new transform hit is not the same as the previous one
            else if (!hitVar.IsNull && hitVar.Value.collider != null && hitVar.Value.collider.transform != previousHit)
            {
                isOverSomething = true;
                var hitTransform = hitVar.Value.collider.transform;
                previousHit = hitTransform;
                new ObjectWasHoveredEvent(origin, hitTransform);
            }
        }
    }
}