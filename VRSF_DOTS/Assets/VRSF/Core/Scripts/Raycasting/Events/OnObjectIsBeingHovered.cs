using UnityEngine;
using VRSF.Core.Raycast;
using VRSF.Core.VRInteractions;

/// <summary>
/// Event raised when an object is hovered by the laser
/// </summary>
public class OnObjectIsBeingHovered : EventCallbacks.Event<OnObjectIsBeingHovered>
{
    /// <summary>
    /// The Origin of the ray that just hovered something
    /// </summary>
    public readonly ERayOrigin RaycastOrigin;

    /// <summary>
    /// The GameObject that was just hovered by the user (must have a collider)
    /// </summary>
    public readonly GameObject HoveredObject;

    /// <summary>
    /// Event raised when an object is hovered by the laser.
    /// </summary>
    /// <param name="raycastOrigin">The Origin of the ray that just hovered something</param>
    /// <param name="objectHovered">The GameObject that was just hovered by the user (must have a collider)</param>
    public OnObjectIsBeingHovered(ERayOrigin raycastOrigin, GameObject objectHovered) : base("Event raised when an object is hovered by the laser.")
    {
        RaycastOrigin = raycastOrigin;
        HoveredObject = objectHovered;

        FireEvent(this);
    }
}