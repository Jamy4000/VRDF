using UnityEngine;
using VRSF.Core.Raycast;

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
    /// <param name="unhoveredObject">The GameObject that was hovered by the user, but isn't anymore (must have a collider)</param>
    public OnStopHoveringObject(ERayOrigin raycastOrigin, GameObject unhoveredObject) : base("Event raised when the user was hovering somethinf with a VR Raycaster, and just stop hovering it")
    {
        RaycastOrigin = raycastOrigin;
        UnhoveredObject = unhoveredObject;

        if (unhoveredObject != null)
        {
            var selectableObject = unhoveredObject.GetComponent<UnityEngine.UI.Selectable>();
            if (selectableObject != null)
                selectableObject.OnDeselect(null);
        }

        HoveringVariablesContainer.SetCurrentHoveredObjects(raycastOrigin, null);

        FireEvent(this);
    }
}