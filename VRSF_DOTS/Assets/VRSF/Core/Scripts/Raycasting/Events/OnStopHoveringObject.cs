using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.VRInteractions;

/// <summary>
/// Event raised when the user was hovering somethinf with a VR Raycaster, and just stop hovering it
/// </summary>
public class OnStopHoveringObject : EventCallbacks.Event<OnStopHoveringObject>
{
    /// <summary>
    /// The Origin of the ray that just hovered something
    /// </summary>
    public readonly ERayOrigin RaycastOrigin;

    /// <summary>
    /// The GameObject that was hovered by the user, but isn't anymore (must have a collider)
    /// </summary>
    public readonly GameObject UnhoveredObject;

    /// <summary>
    /// Event raised when the user was hovering somethinf with a VR Raycaster, and just stop hovering it
    /// </summary>
    /// <param name="raycastOrigin">The Origin of the ray that just hovered something</param>
    /// <param name="objectHovered">The GameObject that was hovered by the user, but isn't anymore (must have a collider)</param>
    public OnStopHoveringObject(ERayOrigin raycastOrigin, GameObject objectHovered) : base("Event raised when the user was hovering somethinf with a VR Raycaster, and just stop hovering it")
    {
        RaycastOrigin = raycastOrigin;
        UnhoveredObject = objectHovered;

        switch (raycastOrigin)
        {
            case ERayOrigin.RIGHT_HAND:
                InteractionVariableContainer.CurrentRightHitPosition = Vector3.zero;
                InteractionVariableContainer.CurrentRightHit = null;
                InteractionVariableContainer.IsOverSomethingRight = false;
                break;
            case ERayOrigin.LEFT_HAND:
                InteractionVariableContainer.CurrentLeftHitPosition = Vector3.zero;
                InteractionVariableContainer.CurrentLeftHit = null;
                InteractionVariableContainer.IsOverSomethingLeft = false;
                break;
            case ERayOrigin.CAMERA:
                InteractionVariableContainer.CurrentGazeHitPosition = Vector3.zero;
                InteractionVariableContainer.CurrentGazeHit = null;
                InteractionVariableContainer.IsOverSomethingGaze = false;
                break;
        }

        FireEvent(this);
    }
}