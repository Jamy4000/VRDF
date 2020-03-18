using UnityEngine;
using VRDF.Core.Raycast;

/// <summary>
/// Event raised when an object is being clicked using the VR Clicker Systems
/// </summary>
public class OnVRClickerIsClicking : EventCallbacks.Event<OnVRClickerIsClicking>
{
    /// <summary>
    /// The Origin of the ray that just clicked something
    /// </summary>
    public readonly ERayOrigin RaycastOrigin;

    /// <summary>
    /// The GameObject that is being clicked by the user
    /// </summary>
    public readonly GameObject ClickedObject;

    /// <summary>
    /// Event raised when an object is being clicked using the VR Clicker Systems
    /// </summary>
    /// <param name="rayOrigin">The Origin of the ray that just clicked something</param>
    /// <param name="objectClicked">The GameObject that is being clicked by the user (must have a collider)</param>
    public OnVRClickerIsClicking(ERayOrigin rayOrigin, GameObject objectClicked) : base("Event raised when an object is being clicked using the VR Clicker Systems.")
    {
        RaycastOrigin = rayOrigin;
        ClickedObject = objectClicked;

        FireEvent(this);
    }
}