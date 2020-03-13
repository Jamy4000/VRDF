using Unity.Entities;
using UnityEngine;
using VRSF.Core.VRInteractions;

namespace VRSF.Core.Raycast
{
    /// <summary>
    /// Check the Raycast of the two controllers and the Camera/Gaze, and reference them in RaycastHit and Ray static classes
    /// Cannot be jobify as we need to be in the MainThread to get the physic from Unity (the Hit and colliders)
    /// </summary>
    public class RaycastHitCalculationsSystem : ComponentSystem
    {
        /// <summary>
        /// Check every frame if the Ray from a controller/the camera is hitting something
        /// </summary>
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity e, ref VRRaycastParameters parameters, ref VRRaycastOutputs raycastOutputs, ref VRRaycastOrigin origin) =>
            {
                var rayHitSomething = Physics.Raycast(raycastOutputs.RayVar, out raycastOutputs.RaycastHitVar.Value, parameters.MaxRaycastDistance, ~parameters.ExcludedLayer);
                raycastOutputs.RaycastHitVar.SetIsNull(!rayHitSomething || raycastOutputs.RaycastHitVar.Value.collider == null);
                Debug.Log("rayHitSomething " + rayHitSomething);
                Debug.Log("raycastOutputs.RaycastHitVar.IsNull " + raycastOutputs.RaycastHitVar.IsNull);

                Vector3 hitPoint = raycastOutputs.RaycastHitVar.IsNull ? Vector3.zero : raycastOutputs.RaycastHitVar.Value.point;
                GameObject hitObject = raycastOutputs.RaycastHitVar.IsNull ? null : raycastOutputs.RaycastHitVar.Value.collider.gameObject;
                // Todo set this somewhere else, after the VRHovering system has run, or find another way to do this
                SetInteractionVariables(origin.RayOrigin, hitPoint, hitObject);
            });
        }

        private void SetInteractionVariables(ERayOrigin rayOrigin, Vector3 hitPoint, GameObject currentRightHit)
        {
            switch (rayOrigin)
            {
                case ERayOrigin.RIGHT_HAND:
                    InteractionVariableContainer.CurrentRightHitPosition = hitPoint;
                    InteractionVariableContainer.CurrentRightHit = currentRightHit;
                    InteractionVariableContainer.IsOverSomethingRight = currentRightHit != null;
                    break;
                case ERayOrigin.LEFT_HAND:
                    InteractionVariableContainer.CurrentLeftHitPosition = hitPoint;
                    InteractionVariableContainer.CurrentLeftHit = currentRightHit;
                    InteractionVariableContainer.IsOverSomethingLeft = currentRightHit != null;
                    break;
                case ERayOrigin.CAMERA:
                    InteractionVariableContainer.CurrentGazeHitPosition = hitPoint;
                    InteractionVariableContainer.CurrentGazeHit = currentRightHit;
                    InteractionVariableContainer.IsOverSomethingGaze = currentRightHit != null;
                    break;
                default:
                    throw new System.Exception();
            }
        }
    }
}