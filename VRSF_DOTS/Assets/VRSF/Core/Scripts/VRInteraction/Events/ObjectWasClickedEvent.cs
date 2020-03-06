using UnityEngine;
using VRSF.Core.Raycast;

/// <summary>
/// Event raised when an object is clicked with the Trigger
/// </summary>
public class ObjectWasClickedEvent : EventCallbacks.Event<ObjectWasClickedEvent>
{
    /// <summary>
    /// The Origin of the ray that just clicked something
    /// </summary>
    public readonly ERayOrigin RayOrigin;

    /// <summary>
    /// The GameObject that was just clicked by the user (must have a collider)
    /// </summary>
    public readonly GameObject ObjectClicked;

    /// <summary>
    /// Event raised when an object is clicked with the Trigger.
    /// </summary>
    /// <param name="rayOrigin">The Origin of the ray that just clicked something</param>
    /// <param name="objectClicked">The GameObject that was just clicked by the user (must have a collider)</param>
    public ObjectWasClickedEvent(ERayOrigin rayOrigin, GameObject objectClicked) : base("Event raised when an object is clicked with the Trigger.")
    {
        RayOrigin = rayOrigin;
        ObjectClicked = objectClicked;

        FireEvent(this);
    }
}